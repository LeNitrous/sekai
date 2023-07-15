// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Hosting;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Mathematics;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// Hosts and manages a <see cref="Game"/>'s lifetime and resources.
/// </summary>
public sealed class Host
{
    /// <summary>
    /// Gets the current host.
    /// </summary>
    public static Host Current => current ?? throw new InvalidOperationException("There is no active host.");

    private static Host? current;

    /// <summary>
    /// Gets the execution mode.
    /// </summary>
    public ExecutionMode ExecutionMode { get; }

    /// <summary>
    /// Gets or sets the ticking mode.
    /// </summary>
    /// <remarks>
    /// When set to <see cref="TickMode.Fixed"/>, the target time between frames is provided by <see cref="UpdateRate"/>.
    /// Otherwise, it is the duration of each elapsed frame.
    /// </remarks>
    public TickMode TickMode { get; set; } = TickMode.Fixed;

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
    /// Gets the primary monitor.
    /// </summary>
    public IMonitor PrimaryMonitor => platform?.PrimaryMonitor ?? throw hostNotLoadedException;

    /// <summary>
    /// Gets all available monitors.
    /// </summary>
    public IEnumerable<IMonitor> Monitors => platform?.Monitors ?? throw hostNotLoadedException;

    /// <summary>
    /// The host's logger.
    /// </summary>
    internal readonly Logger Logger = new();

    /// <summary>
    /// The host's options passed during construction.
    /// </summary>
    internal readonly HostOptions Options;

    private Game? game;
    private Platform? platform;
    private LogWriterText? fileOut;
    private LogWriterConsole? termOut;
    private HostState state;
    private Waitable waitable;
    private TimeSpan msPerUpdate = TimeSpan.FromSeconds(1 / 120.0);
    private TimeSpan accumulated;
    private TimeSpan currentTime;
    private TimeSpan elapsedTime;
    private TimeSpan previousTime;
    private bool hasShownWindow;
    private ExceptionDispatchInfo? info;
    private readonly object sync = new();
    private readonly Stopwatch stopwatch = new();
    private readonly MergedInputSource inputContext = new();
    private static readonly TimeSpan wait_threshold = TimeSpan.FromMilliseconds(2);

    private Host(HostOptions? options = null)
    {
        Options = options ?? new();
        TickMode = Options.TickMode;
        UpdateRate = Options.UpdateRate;
        ExecutionMode = Options.ExecutionMode;
    }

    /// <summary>
    /// Runs the game.
    /// </summary>
    /// <typeparam name="T">The game to run.</typeparam>
    public static void Run<T>(HostOptions? options = null)
        where T : Game, new()
    {
        try
        {
            RunAsync<T>(options).Wait();
        }
        catch (AggregateException e)
        {
            ExceptionDispatchInfo.Throw(e.InnerException ?? e);
        }
    }

    /// <summary>
    /// Runs the game asynchronously.
    /// </summary>
    /// <typeparam name="T">The game to run.</typeparam>
    /// <param name="token">The cancellation token when canceled closes the game.</param>
    /// <returns>A task that represents the game's main execution loop.</returns>
    public static async Task RunAsync<T>(HostOptions? options = null, CancellationToken token = default)
        where T : Game, new()
    {
        var game = new T();
        var host = new Host(options);
        await host.run(game, token);
    }

    private async Task run(Game game, CancellationToken token = default)
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

        platform = platformProvider.CreatePlatform(Options);

        game.Host = this;
        game.Options = Options;
        game.Storage = platform.CreateStorage();

        if (game.Storage is MountableStorage mount && mount.IsMounted(Storage.User))
        {
            Logger.AddOutput(fileOut = new LogWriterText(mount.Open("/user/runtime.log", FileMode.Create, FileAccess.Write)));
        }

        if (RuntimeInfo.IsDebug)
        {
            Logger.AddOutput(termOut = new LogWriterConsole());
        }

        var asm = Assembly.GetEntryAssembly()?.GetName();

        Logger.Log("--------------------------------------------------------");
        Logger.Log("  Logging for {0}", Environment.UserName);
        Logger.Log("  Running {0} {1} on .NET {2}", asm?.Name, asm?.Version, Environment.Version);
        Logger.Log("  Environment: {0}, ({1}) {2} cores", RuntimeInfo.OS, Environment.OSVersion, Environment.ProcessorCount);
        Logger.Log("--------------------------------------------------------");

        game.Window = platform.CreateWindow();
        game.Window.Size = Options.WindowSize;
        game.Window.Title = Options.Name;
        game.Window.State = WindowState.Minimized;
        game.Window.Border = Options.WindowBorder;
        game.Window.Closed += () => exit(true);

        if (game.Window is IHasSuspend suspendable)
        {
            suspendable.Resumed += resumed;
            suspendable.Suspend += suspend;
        }

        if (game.Window is IHasRestart restartable)
        {
            restartable.Restart += restart;
        }

        setHostState(HostState.Loading);

        Task? gameLoop = null;

        if (ExecutionMode == ExecutionMode.MultiThread)
        {
            gameLoop = Task.Factory.StartNew(runGameLoop, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        while (game.Window.Exists)
        {
            if (info is not null)
            {
                info.Throw();
                break;
            }

            platform.DoEvents();
            game.Window.DoEvents();

            if (ExecutionMode == ExecutionMode.SingleThread && !runTick())
            {
                break;
            }

            if (!hasShownWindow && State == HostState.Running)
            {
                game.Window.State = WindowState.Normal;
                game.Window.Visible = true;
                game.Window.Focus();
                hasShownWindow = true;
            }
        }

        if (gameLoop is not null)
        {
            await gameLoop;
        }

        termOut?.Dispose();
        fileOut?.Dispose();

        game.Window.Dispose();
        platform?.Dispose();

        current = null;
    }

    /// <summary>
    /// Exits the game.
    /// </summary>
    public void Exit()
    {
        exit(false);
    }

    private void exit(bool userTriggered)
    {
        if (State == HostState.Exiting)
        {
            return;
        }

        if (userTriggered)
        {
            setHostState(HostState.Exiting);
        }
        else
        {
            game!.Window.Close();
        }
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

    private Task runGameLoop()
    {
        try
        {
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

        game.Graphics?.MakeCurrent();

        switch (State)
        {
            case HostState.Loading:
                {
                    if (game.Window is IInputSource windowInput)
                    {
                        inputContext.Add(windowInput);
                    }

                    if (platform is IInputSource platformInput)
                    {
                        inputContext.Add(platformInput);
                    }

                    game.Input = new(inputContext);
                    game.Audio = audioProvider.CreateAudio();

                    Logger.Log("{0} Initialized", game.Audio.API);
                    Logger.Log("     Device: {0}", game.Audio.Device);
                    Logger.Log("    Version: {0}", game.Audio.Version);

                    game.Graphics = graphicsProvider.CreateGraphics(game.Window);

                    Logger.Log("{0} Initialized", game.Graphics.API);
                    Logger.Log("     Device: {0}", game.Graphics.Device);
                    Logger.Log("     Vendor: {0}", game.Graphics.Vendor);
                    Logger.Log("    Version: {0}", game.Graphics.Version);

                    accumulated = TimeSpan.Zero;
                    currentTime = TimeSpan.Zero;
                    elapsedTime = TimeSpan.Zero;
                    previousTime = TimeSpan.Zero;

                    stopwatch.Start();

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

                    game.Graphics!.SetScissor(new Rectangle(Point.Zero, game.Window.Size));
                    game.Graphics!.SetViewport(new Rectangle(Point.Zero, game.Window.Size));

                    game.Draw();

                    game.Graphics!.Present();

                    return true;
                }

            case HostState.Resuming:
                {
                    game.Audio.Process();
                    stopwatch.Start();

                    setHostState(HostState.Running);
                    return true;
                }

            case HostState.Pausing:
                {
                    game.Audio.Suspend();
                    stopwatch.Stop();

                    setHostState(HostState.Paused);
                    return true;
                }

            case HostState.Exiting:
            case HostState.Reloading:
                {
                    game.Unload();

                    game.Input?.Dispose();
                    game.Input = null!;

                    game.Audio?.Dispose();
                    game.Audio = null!;

                    game.Graphics?.Dispose();
                    game.Graphics = null!;

                    inputContext.Clear();

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

    private static readonly Exception hostNotLoadedException = new InvalidOperationException("The host has not yet loaded");
    private static readonly IAudioProvider audioProvider;
    private static readonly IPlatformProvider platformProvider;
    private static readonly IGraphicsProvider graphicsProvider;

    static Host()
    {
        audioProvider = getProvider<AudioProviderAttribute, IAudioProvider>(provider_names_audio);
        platformProvider = getProvider<PlatformProviderAttribute, IPlatformProvider>(provider_names_platform);
        graphicsProvider = getProvider<GraphicsProviderAttribute, IGraphicsProvider>(provider_names_graphics);
    }

    private static U getProvider<T, U>(string[] assemblyNames)
        where T : Attribute, IProviderAttribute
        where U : class
    {
        foreach (string assemblyName in assemblyNames)
        {
            if (tryLoad<T, U>(assemblyName, out var provider))
            {
                return provider;
            }
        }

        throw new InvalidOperationException("Failed to load provider.");
    }

    private static bool tryLoad<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T, U>(string assemblyName, [NotNullWhen(true)] out U? provider)
        where T : Attribute, IProviderAttribute
        where U : class
    {
        provider = null;

        try
        {
            var asm = Assembly.Load(assemblyName);
            var att = asm.GetCustomAttribute<T>();

            if (att is null)
            {
                return false;
            }

            if (!typeof(U).IsAssignableFrom(att.Type))
            {
                return false;
            }

            provider = (U)Activator.CreateInstance(att.Type)!;

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static readonly string[] provider_names_platform = new string[]
    {
        "Sekai.Desktop",
        "Sekai.Headless",
    };

    private static readonly string[] provider_names_graphics = new string[]
    {
        "Sekai.OpenGL",
        "Sekai.Headless",
    };

    private static readonly string[] provider_names_audio = new string[]
    {
        "Sekai.OpenAL",
        "Sekai.Headless",
    };
}
