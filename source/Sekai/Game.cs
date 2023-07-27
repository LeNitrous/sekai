// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Mathematics;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// The entry point for Sekai.
/// </summary>
public abstract class Game
{
    /// <summary>
    /// Gets the ticking mode.
    /// </summary>
    public TickMode TickMode { get; }

    /// <summary>
    /// Gets the execution mode.
    /// </summary>
    public ExecutionMode ExecutionMode { get; }

    /// <summary>
    /// Gets or sets the update rate (in seconds) of the game when <see cref="TickMode"/> is <see cref="TickMode.Fixed"/>.
    /// </summary>
    /// <remarks>
    /// The returned value may be an approximation of the originally set value.
    /// </remarks>
    public double UpdateRate
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
    /// The game view.
    /// </summary>
    protected IView View { get; }

    /// <summary>
    /// The game logger.
    /// </summary>
    protected ILogger Logger { get; }

    /// <summary>
    /// The game storage.
    /// </summary>
    protected Storage Storage { get; }

    /// <summary>
    /// The audio device.
    /// </summary>
    protected AudioDevice Audio { get; }

    /// <summary>
    /// The input manager.
    /// </summary>
    protected IInputSource Input { get; }

    /// <summary>
    /// The graphics device.
    /// </summary>
    protected GraphicsDevice Graphics { get; }

    private GameState state;
    private TimeSpan msPerUpdate;
    private TimeSpan accumulated;
    private TimeSpan currentTime;
    private TimeSpan elapsedTime;
    private TimeSpan previousTime;
    private ExceptionDispatchInfo? info;
    private readonly object syncLock = new();
    private readonly Waitable waitable = new();
    private readonly Stopwatch stopwatch = new();
    private const string default_window_title = "Sekai";
    private static readonly TimeSpan wait_threshold = TimeSpan.FromMilliseconds(2);
    private static readonly TimeSpan time_allowable = TimeSpan.FromMilliseconds(20);

    public Game(GameOptions options)
    {
        TickMode = options.TickMode;
        UpdateRate = options.UpdateRate;
        ExecutionMode = options.ExecutionMode;

        Storage = options.Storage.Create();
        Logger = options.Logger.Create(Storage);

        View = options.View.Create();
        View.Closing += () => setGameState(GameState.Exiting);
        View.Suspend += () => setGameState(GameState.Pausing);
        View.Resumed += () => setGameState(GameState.Resuming);

        if (View is IWindow window && options.View is GameOptions.WindowOptions windowOptions)
        {
            window.Size = windowOptions.Size;
            window.Title = string.IsNullOrEmpty(windowOptions.Title) ? Assembly.GetEntryAssembly()?.GetName().Name ?? default_window_title : windowOptions.Title;
            window.Border = windowOptions.Border;
            window.Visible = false;
            window.MinimumSize = windowOptions.MinimumSize;
            window.MaximumSize = windowOptions.MaximumSize;

            if (windowOptions.Position.X != -1 && windowOptions.Position.Y != -1)
            {
                window.Position = windowOptions.Position;
            }
        }

        if (View is IInputSource viewInputSource)
        {
            options.Input.AddSource(viewInputSource);
        }

        Input = options.Input.Create();

        Audio = options.Audio.CreateDevice();
        Audio.Device = options.Audio.Device;

        Graphics = options.Graphics.CreateDevice(View);
        Graphics.SyncMode = options.Graphics.SyncMode;
    }

    /// <summary>
    /// Runs the game asynchronously.
    /// </summary>
    /// <param name="token">The cancellation token when canceled closes the game.</param>
    /// <returns>A task that represents the game's main execution loop.</returns>
    public async Task RunAsync(CancellationToken token = default)
    {
        if (state != GameState.Ready)
        {
            throw new InvalidOperationException("This game instance has already begun running.");
        }

        setGameState(GameState.Loading);

        if (token.CanBeCanceled)
        {
            token.Register(Exit);
        }

        var gameLoop = Task.CompletedTask;

        if (ExecutionMode == ExecutionMode.MultiThread)
        {
            gameLoop = Task.Factory.StartNew(runGameLoop, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }
        else
        {
            View.Tick += runFrame;
        }

        View.Tick += runViewEvents;

        if (View is IWindow window)
        {
            window.Run();
        }

        await gameLoop;
    }

    /// <summary>
    /// Runs the game on the current thread.
    /// </summary>
    public void Run()
    {
        try
        {
            RunAsync().Wait();
        }
        catch (AggregateException e)
        {
            ExceptionDispatchInfo.Throw(e.InnerException ?? e);
        }
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        if (state == GameState.Ready)
        {
            throw new InvalidOperationException("The game has not yet initialized.");
        }

        View.Close();
    }

    /// <summary>
    /// Called once after the game starts running.
    /// </summary>
    protected virtual void Load()
    {
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void Draw()
    {
    }

    /// <summary>
    /// Called every frame when <see cref="TickMode.Variable"/> or possibly multiple times when <see cref="TickMode.Fixed"/>.
    /// </summary>
    /// <param name="elapsed">The time between frames.</param>
    protected virtual void Update(TimeSpan elapsed)
    {
    }

    /// <summary>
    /// Called once before the game closes.
    /// </summary>
    protected virtual void Unload()
    {
    }

    /// <summary>
    /// Called when game execution has been suspended.
    /// </summary>
    protected virtual void Suspend()
    {
    }

    /// <summary>
    /// Called when game execution resumes.
    /// </summary>
    protected virtual void Resumed()
    {
    }

    private void runViewEvents()
    {
        info?.Throw();
    }

    private Task runGameLoop()
    {
        do
        {
            try
            {
                runFrame();
            }
            catch (Exception e)
            {
                info = ExceptionDispatchInfo.Capture(e);
                throw;
            }
        }
        while (state != GameState.Exited);

        return Task.CompletedTask;
    }

    private void runFrame()
    {
        Graphics.MakeCurrent();

        switch (state)
        {
            case GameState.Loading:
                {
                    initialize();
                    Load();
                    tick();

                    if (View is IWindow window)
                    {
                        window.Visible = true;
                    }

                    setGameState(GameState.Running);
                    break;
                }

            case GameState.Running:
                {
                    tick();
                    break;
                }

            case GameState.Pausing:
                {
                    Audio.Suspend();
                    stopwatch.Stop();
                    Suspend();
                    setGameState(GameState.Paused);
                    break;
                }

            case GameState.Resuming:
                {
                    Audio.Process();
                    stopwatch.Start();
                    Resumed();
                    setGameState(GameState.Running);
                    break;
                }

            case GameState.Exiting:
                {
                    Unload();
                    shutdown();
                    setGameState(GameState.Exited);
                    break;
                }
        }
    }

    private void initialize()
    {
        var asm = Assembly.GetEntryAssembly()?.GetName();

        Logger.Log("--------------------------------------------------------");
        Logger.Log("  Logging for {0}", Environment.UserName);
        Logger.Log("  Running {0} {1} on .NET {2}", asm?.Name, asm?.Version, Environment.Version);
        Logger.Log("  Environment: {0}, ({1}) {2} cores", RuntimeInfo.OS, Environment.OSVersion, Environment.ProcessorCount);
        Logger.Log("--------------------------------------------------------");

        Logger.Log("{0} Initialized", Audio.API);
        Logger.Log("     Device: {0}", Audio.Device);
        Logger.Log("    Version: {0}", Audio.Version);

        Logger.Log("{0} Initialized", Graphics.API);
        Logger.Log("     Device: {0}", Graphics.Device);
        Logger.Log("     Vendor: {0}", Graphics.Vendor);
        Logger.Log("    Version: {0}", Graphics.Version);
    }

    private void shutdown()
    {
        View.Dispose();
        Audio.Dispose();
        Graphics.Dispose();
    }

    private void tick()
    {
        if (!stopwatch.IsRunning)
        {
            stopwatch.Start();
        }

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
                Update(msPerUpdate);
            }
        }
        else
        {
            if (elapsedTime > time_allowable)
            {
                Logger.Trace("Time exceeded allowable delta. (was {0:0.0000}ms)", elapsedTime.TotalMilliseconds);
            }

            Update(elapsedTime);
        }

        Graphics.SetScissor(new Rectangle(Point.Zero, View.Size));
        Graphics.SetViewport(new Rectangle(Point.Zero, View.Size));

        Draw();

        Graphics.Present();
    }

    private void setGameState(GameState next)
    {
        lock (syncLock)
        {
            if (!isValidTransition(state, next))
            {
                throw new InvalidOperationException($"Invalid state transition {state} -> {next}.");
            }

            Logger.Trace("Game changed state {0} -> {1}.", state, next);

            state = next;
        }
    }

    private static bool isValidTransition(GameState current, GameState next)
    {
        switch (next)
        {
            case GameState.Loading:
                return current is GameState.Ready;

            case GameState.Running:
                return current is GameState.Loading or GameState.Resuming;

            case GameState.Pausing:
                return current is GameState.Running;

            case GameState.Paused:
                return current is GameState.Pausing;

            case GameState.Resuming:
                return current is GameState.Paused;

            case GameState.Exiting:
                return current is GameState.Running or GameState.Paused or GameState.Resuming;

            case GameState.Exited:
                return current is GameState.Exiting;

            default:
                return false;
        }
    }

    private enum GameState
    {
        Ready,
        Loading,
        Running,
        Pausing,
        Paused,
        Resuming,
        Exiting,
        Exited,
    }
}
