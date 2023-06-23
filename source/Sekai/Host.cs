// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework;
using Sekai.Framework.Audio;
using Sekai.Framework.Graphics;
using Sekai.Framework.Input;
using Sekai.Framework.Logging;
using Sekai.Framework.Mathematics;
using Sekai.Framework.Storages;
using Sekai.Framework.Windowing;

namespace Sekai;

/// <summary>
/// Hosts and manages a <see cref="Game"/>'s lifetime and resources.
/// </summary>
public abstract class Host
{
    /// <summary>
    /// Gets the current host.
    /// </summary>
    public static Host Current => current ?? throw new InvalidOperationException("There is no active host.");

    private static Host? current;

    /// <summary>
    /// Gets or sets the ticking mode.
    /// </summary>
    /// <remarks>
    /// When set to <see cref="TickMode.Fixed"/>, the target time between frames is provided by <see cref="UpdatePerSecond"/>.
    /// Otherwise, it is the duration of each elapsed frame.
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

    /// <summary>
    /// An enumeration of connected monitors.
    /// </summary>
    public abstract IEnumerable<IMonitor> Monitors { get; }

    internal Logger Logger => logger ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal IWindow Window => window ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal InputState Input => input ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal AudioDevice Audio => audio ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal GraphicsDevice Graphics => graphics ?? throw new InvalidOperationException("The host has not yet loaded.");
    internal Storage Storage => storage ?? throw new InvalidOperationException("The host has not yet loaded.");

    private Game? game;
    private Logger? logger;
    private IWindow? window;
    private InputState? input;
    private AudioDevice? audio;
    private GraphicsDevice? graphics;
    private Storage? storage;
    private LogWriterConsole? termOut;
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
        try
        {
            RunAsync(game).Wait();
        }
        catch (AggregateException e)
        {
            ExceptionDispatchInfo.Throw(e.InnerException ?? e);
        }
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
            throw new InvalidOperationException("This host is currently running a game.");
        }

        if (current is not null)
        {
            throw new InvalidOperationException("Cannot run a game with an active host.");
        }

        current = this;

        if (token.CanBeCanceled)
        {
            token.Register(Exit);
        }

        this.game = game;

        Initialize();

        setHostState(HostState.Loading);

        var gameLoop = Task.Factory.StartNew(runGameLoop, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        while (Window.Exists)
        {
            if (info is not null)
            {
                info.Throw();
                break;
            }

            Update();
        }

        await gameLoop;

        Shutdown();

        current = null;
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        setHostState(HostState.Exiting);
    }

    private void suspend()
    {
        if (State == HostState.Paused)
        {
            return;
        }

        setHostState(HostState.Pausing);
    }

    private void resumed()
    {
        if (State != HostState.Paused)
        {
            return;
        }

        setHostState(HostState.Resuming);
    }

    private void restart()
    {
        setHostState(HostState.Reloading);
    }

    /// <summary>
    /// Called before running the game to perform initialization.
    /// </summary>
    protected virtual void Initialize()
    {
        logger = new();

        if (RuntimeInfo.IsDebug)
        {
            logger.AddOutput(termOut = new LogWriterConsole());
        }

        var asm = Assembly.GetEntryAssembly()?.GetName();

        logger.Log("--------------------------------------------------------");
        logger.Log("  Logging for {0}", Environment.UserName);
        logger.Log("  Running {0} {1} on .NET {2}", asm?.Name, asm?.Version, Environment.Version);
        logger.Log("  Environment: {0}, ({1}) {2} cores", RuntimeInfo.OS, Environment.OSVersion, Environment.ProcessorCount);
        logger.Log("--------------------------------------------------------");

        storage = CreateStorage();

        window = CreateWindow();
        window.Title = $"Sekai{(asm?.Name is not null ? $" (running {asm.Name})" : string.Empty)}";
        window.State = WindowState.Minimized;
        window.Closed += Exit;

        if (window is IHasSuspend suspendable)
        {
            suspendable.Resumed += resumed;
            suspendable.Suspend += suspend;
        }

        if (window is IHasRestart restartable)
        {
            restartable.Restart += restart;
        }
    }

    /// <summary>
    /// Called every frame on the main thread.
    /// </summary>
    protected virtual void Update()
    {
        Window.DoEvents();
    }

    /// <summary>
    /// Called every frame on the game thread regardless of state.
    /// </summary>
    protected virtual void Perform()
    {
    }

    /// <summary>
    /// Called before closing the game to perform shutdown.
    /// </summary>
    protected virtual void Shutdown()
    {
        Window.Dispose();
        termOut?.Dispose();
    }

    /// <summary>
    /// Creates a window for this host.
    /// </summary>
    protected abstract IWindow CreateWindow();

    /// <summary>
    /// Creates an input context for this host.
    /// </summary>
    protected abstract IInputContext CreateInput(IWindow window);

    /// <summary>
    /// Creates the audio device for this host.
    /// </summary>
    protected abstract AudioDevice CreateAudio();

    /// <summary>
    /// Creates storage for this host.
    /// </summary>
    protected abstract Storage CreateStorage();

    /// <summary>
    /// Creates the graphics device for this host.
    /// </summary>
    /// <param name="window">The window backing the graphics device.</param>
    protected abstract GraphicsDevice CreateGraphics(IWindow window);

    private ExceptionDispatchInfo? info;

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

            while (runTick()) ;
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

        // Perform();

        switch (State)
        {
            case HostState.Loading:
                {
                    input = new(CreateInput(Window));
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

                    Graphics.SetScissor(new Rectangle(Point.Zero, Window.Size));
                    Graphics.SetViewport(new Rectangle(Point.Zero, Window.Size));

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

                    input?.Dispose();
                    input = null;

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
