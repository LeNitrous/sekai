// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework.Audio;
using Sekai.Framework.Logging;
using Sekai.Framework.Storages;
using Sekai.Framework.Graphics;
using Sekai.Framework.Platform.Input;
using Sekai.Framework.Platform.Windowing;

namespace Sekai;

/// <summary>
/// Hosts and manages a <see cref="Game"/>'s lifetime and resources.
/// </summary>
public abstract class Host
{
    /// <summary>
    /// Gets or sets the ticking logic performed by <see cref="Tick"/>.
    /// </summary>
    /// <remarks>
    /// When set to <see cref="TickMode.Fixed"/>, the target time between frames is provided by <see cref="UpdatePerSecond"/>.
    /// </remarks>
    public TickMode TickMode { get; set; } = TickMode.Fixed;

    /// <summary>
    /// Gets or sets the update rate of the game when <see cref="TickMode"/> is <see cref="TickMode.Fixed"/>.
    /// </summary>
    /// <remarks>
    /// The returned value may be an approximation of the originally set value.
    /// </remarks>
    public double UpdatePerSecond
    {
        get => 1000 / msPerUpdate.Milliseconds;
        set
        {
            if (value <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be greater than zero.");
            }

            msPerUpdate = TimeSpan.FromSeconds(1 / value);
        }
    }

    /// <summary>
    /// The host's state.
    /// </summary>
    public HostState State
    {
        get
        {
            lock (sync)
            {
                return state;
            }
        }
    }

    internal Logger Logger => logger ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal IWindow Window => window ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal AudioDevice Audio => audio ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal GraphicsDevice Graphics => graphics ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal MountableStorage Storage => storage ?? throw new InvalidOperationException("The host has not yet loaded.");

    private Game? game;
    private Logger? logger;
    private IWindow? window;
    private AudioDevice? audio;
    private GraphicsDevice? graphics;
    private MountableStorage? storage;
    private HostState state;
    private Waitable waitable;
    private TimeSpan msPerUpdate = TimeSpan.FromSeconds(1 / 120.0);
    private TimeSpan accumulated;
    private TimeSpan currentTime;
    private TimeSpan elapsedTime;
    private TimeSpan previousTime;
    private readonly object sync = new();
    private readonly Stopwatch stopwatch = new();
    private static readonly TimeSpan wait_threshold = TimeSpan.FromMilliseconds(2);

    /// <summary>
    /// Runs the game.
    /// </summary>
    /// <param name="game">The game to run.</param>
    public void Run(Game game)
    {
        RunAsync(game).Wait();
    }

    /// <summary>
    /// Runs the game asynchronously.
    /// </summary>
    /// <param name="game">The game to run.</param>
    /// <param name="token">The cancellation token when canceled closes the game.</param>
    /// <returns>A task that represents the game's main execution loop.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is already a game being ran.</exception>
    public async Task RunAsync(Game game, CancellationToken token = default)
    {
        if (this.game is not null)
        {
            throw new InvalidOperationException();
        }

        if (token.CanBeCanceled)
        {
            token.Register(Exit);
        }

        if (RuntimeInfo.IsDebug)
        {
            HotReloadCallbackReceiver.OnUpdate += handleHotReload;
        }

        storage = new();
        storage.Mount("/game/", CreateStorage(MountPoint.Game));
        storage.Mount("/data/", CreateStorage(MountPoint.Data));
        storage.Mount("/temp/", CreateStorage(MountPoint.Temp));

        var termOut = new LogWriterConsole();
        var fileOut = new LogWriterStream(storage.Open("/game/runtime.log", FileMode.OpenOrCreate, FileAccess.Write));

        logger = new();
        logger.AddOutput(termOut);
        logger.AddOutput(fileOut);

        var asm = Assembly.GetEntryAssembly()?.GetName();

        logger.Log("--------------------------------------------------------");
        logger.Log("  Logging for {0}", Environment.UserName);
        logger.Log("  Running {0} {1} on .NET {2}", asm?.Name, asm?.Version, Environment.Version);
        logger.Log("  Environment: {0}, ({1}) {2} cores", RuntimeInfo.OS, Environment.OSVersion, Environment.ProcessorCount);
        logger.Log("--------------------------------------------------------");

        this.game = game;

        setHostState(HostState.Loading);

        window = CreateWindow();
        window.Title = $"Sekai{(asm?.Name is not null ? $" (running {asm.Name})" : string.Empty)}";
        window.State = WindowState.Minimized;
        window.Closed += Exit;

        var mainLoop = Task.Factory.StartNew(runMainLoop, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        var gameLoop = Task.Factory.StartNew(runGameLoop, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        await mainLoop;
        await gameLoop;

        window.Dispose();

        fileOut.Dispose();
        termOut.Dispose();
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        setHostState(HostState.Exiting);
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    protected void Pause()
    {
        setHostState(HostState.Pausing);
    }

    /// <summary>
    /// Resumes the game.
    /// </summary>
    protected void Resume()
    {
        if (State != HostState.Paused)
        {
            return;
        }

        setHostState(HostState.Resuming);
    }

    /// <summary>
    /// Reloads the game.
    /// </summary>
    protected void Reload()
    {
        setHostState(HostState.Reloading);
    }

    /// <summary>
    /// Creates a window for this host.
    /// </summary>
    protected abstract IWindow CreateWindow();

    /// <summary>
    /// Creates the audio device for this host.
    /// </summary>
    /// <returns></returns>
    protected abstract AudioDevice CreateAudio();

    /// <summary>
    /// Creates storage for this host.
    /// </summary>
    /// <param name="point">The mounting point.</param>
    protected abstract Storage CreateStorage(MountPoint point);

    /// <summary>
    /// Creates the graphics device for this host.
    /// </summary>
    /// <param name="window">The window backing the graphics device.</param>
    protected abstract GraphicsDevice CreateGraphics(IWindow window);

    private void handleHotReload(Type[]? types)
    {
        Logger.Debug("Hot reload requested. Reloading the game...");
        Reload();
    }

    private ExceptionDispatchInfo? info;

    private Task runMainLoop()
    {
        while (Window.Exists)
        {
            if (info is not null)
            {
                info.Throw();
                break;
            }

            Window.DoEvents();
        }

        return Task.CompletedTask;
    }

    private Task runGameLoop()
    {
        try
        {
            if (runTick())
            {
                Window.State = WindowState.Normal;
                Window.Visible = true;
                Window.Focus();
            }

            do
            {
                if (window is not IInputSource source)
                {
                    continue;
                }

                while(source.PumpEvent(out var data))
                {

                }
            }
            while (runTick());
        }
        catch (Exception e)
        {
            info = ExceptionDispatchInfo.Capture(e);
            throw;
        }

        return Task.CompletedTask;
    }

    private bool runTick()
    {
        if (game is null)
        {
            return false;
        }

        graphics?.MakeCurrent();

        switch (State)
        {
            case HostState.Loading:
                {
                    audio = CreateAudio();

                    Logger.Log("{0} Initialized", audio.API);
                    Logger.Log("     Device: {0}", audio.Device);
                    Logger.Log("    Version: {0}", audio.Version);

                    graphics = CreateGraphics(Window);

                    Logger.Log("{0} Initialized", graphics.API);
                    Logger.Log("     Device: {0}", graphics.Device);
                    Logger.Log("     Vendor: {0}", graphics.Vendor);
                    Logger.Log("    Version: {0}", graphics.Version);

                    accumulated = TimeSpan.Zero;
                    currentTime = TimeSpan.Zero;
                    elapsedTime = TimeSpan.Zero;
                    previousTime = TimeSpan.Zero;

                    stopwatch.Start();

                    game.Host = this;
                    game.Load();
                    setHostState(HostState.Running);

                    return true;
                }

            case HostState.Running:
                {
                    bool retry = false;

                    do
                    {
                        currentTime = stopwatch.Elapsed;
                        elapsedTime = currentTime - previousTime;
                        accumulated += elapsedTime;
                        previousTime = currentTime;

                        if (TickMode == TickMode.Fixed && accumulated < msPerUpdate)
                        {
                            var duration = msPerUpdate - accumulated;

                            // Timers are at the mercy of the operating system's scheduler. Thus sleep() can be
                            // very innacurate. It can end earlier or later than the requested time. It depends
                            // when the scheduler gives back control to our application.

                            // To save on resources, we avoid using a spin lock for the entire duration of the
                            // wait. We can use sleep() first to give back control to the scheduler and free up
                            // CPU usage for the duration less the threshold we allow it to.
                            if (duration >= wait_threshold)
                            {
                                waitable.Wait(duration - wait_threshold);
                            }

                            // After the wait, we still have a bit more time remaining. To accurately finish off,
                            // we use a spin lock. Because of the earlier wait, we have less iterations to do in
                            // this spin lock.
                            while (duration > stopwatch.Elapsed - currentTime) ;

                            retry = true;
                        }
                        else
                        {
                            retry = false;
                        }
                    }
                    while (retry);

                    if (TickMode == TickMode.Fixed)
                    {
                        while (accumulated >= msPerUpdate)
                        {
                            accumulated -= msPerUpdate;
                            game.Update(msPerUpdate);
                        }
                    }
                    else
                    {
                        game.Update(elapsedTime);
                    }

                    Graphics.SetScissor(new Rectangle(Point.Empty, Window.Size));
                    Graphics.SetViewport(new Rectangle(Point.Empty, Window.Size));
                    Graphics.Clear(new ClearInfo(Color.CornflowerBlue));

                    game.Draw();

                    Graphics.Present();

                    return true;
                }

            case HostState.Resuming:
                {
                    Audio.Process();
                    stopwatch.Start();

                    setHostState(HostState.Running);
                    return true;
                }

            case HostState.Pausing:
                {
                    Audio.Suspend();
                    stopwatch.Stop();

                    setHostState(HostState.Paused);
                    return true;
                }

            case HostState.Exiting:
            case HostState.Reloading:
                {
                    game.Unload();
                    game.Host = null!;

                    audio?.Dispose();
                    audio = null;

                    graphics?.Dispose();
                    graphics = null;

                    if (state == HostState.Reloading)
                    {
                        setHostState(HostState.Loading);
                        return true;
                    }
                    else
                    {
                        setHostState(HostState.Exited);
                        return false;
                    }
                }

            default:
                throw new InvalidOperationException("Host is in an invalid state.");
        }
    }

    private void setHostState(HostState next)
    {
        lock (sync)
        {
            if (!isValidTransition(state, next))
            {
                throw new InvalidOperationException($"Invalid state transition {state} -> {next}.");
            }

            Logger.Trace("Host changed state {0} -> {1}.", state, next);

            state = next;
        }
    }

    private static bool isValidTransition(HostState current, HostState next)
    {
        switch (next)
        {
            case HostState.Loading:
                return current is HostState.Idle or HostState.Reloading;

            case HostState.Running:
                return current is HostState.Loading or HostState.Resuming;

            case HostState.Paused:
                return current is HostState.Pausing;

            case HostState.Resuming:
                return current is HostState.Paused;

            case HostState.Exited:
                return current is HostState.Exiting;

            case HostState.Exiting:
            case HostState.Reloading:
                return current is HostState.Running or HostState.Pausing or HostState.Paused or HostState.Resuming or HostState.Resumed;

            default:
                return false;
        }
    }
}
