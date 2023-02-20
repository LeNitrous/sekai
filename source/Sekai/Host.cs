// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using Sekai.Threading;
using System.Collections.Generic;
using Sekai.Graphics;
using Sekai.Audio;
using Sekai.Input;
using Sekai.Windowing;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using System.Threading;
using Sekai.Allocation;
using Sekai.Storages;
using Sekai.Logging;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Sekai;

public sealed class Host : IHost, IHostStartup
{
    /// <summary>
    /// The currently active host instance.
    /// </summary>
    public static Host Current => current ?? throw new InvalidOperationException("There is no host currently active.");

    /// <summary>
    /// The host's threads.
    /// </summary>
    public readonly HostThreads Threads;

    /// <summary>
    /// The host's services.
    /// </summary>
    public readonly ServiceLocator Services;

    private bool hasShownWindow;
    private ExecutionMode executionMode = ExecutionMode.MultiThread;
    private ExecutionMode? activeExecutionMode;
    private Game? game;
    private double updatePerSecond = WorkerThread.DEFAULT_UPDATE_PER_SECOND;
    private static Host? current;
    private event Func<Exception, bool>? onException;
    private readonly GameOptions options;
    private readonly Time time = new();
    private readonly Func<Game> creator;
    private readonly Logger logger;
    private readonly Surface surface;
    private readonly Storage storage;
    private readonly AudioContext audio;
    private readonly InputContext input;
    private readonly GraphicsContext graphics;
    private readonly Action<IHost>? onInitAction;
    private readonly Action<IHost>? onExitAction;
    private readonly object syncLock = new();
    private readonly List<WorkerThread> workers = new();

    internal Host(Func<Game> creator, GameOptions options, Logger logger, Surface surface, Storage storage, AudioContext audio, InputContext input, GraphicsContext graphics, ServiceCollection services, IReadOnlyDictionary<string, WorkerThread> threads, Action<IHost>? onInitAction, Action<IHost>? onExitAction)
    {
        this.audio = audio;
        this.input = input;
        this.logger = logger;
        this.options = options;
        this.storage = storage;
        this.surface = surface;
        this.creator = creator;
        this.graphics = graphics;
        this.onInitAction = onInitAction;
        this.onExitAction = onExitAction;

        Threads = new(threads["Main"], threads["Game"], threads["Audio"]);
        Threads.Game.OnExit += onUpdateExit;
        Threads.Game.OnStart += onUpdateInit;
        Threads.Game.OnNewFrame += onUpdateLoop;
        Threads.Audio.OnExit += audio.Dispose;
        Threads.Audio.OnNewFrame += audio.Update;
        workers.AddRange(threads.Values);

        services.AddConstant(time);
        services.AddConstant<IHost>(this);
        Services = services.CreateLocator();

        this.surface.OnClose += onSurfaceClose;
        this.surface.OnUpdate += doWork;
        this.surface.OnStateChanged += onSurfaceStateChange;
    }

    private void run()
    {
        if (current is not null)
            throw new InvalidOperationException("There is a host instance already running.");

        current = this;

        logger.Info(new string('-', 60));
        logger.Info($"Logging for {Environment.UserName}");
        logger.Info($"Running {options.Name} {Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString() ?? "unknown"} on {RuntimeInformation.FrameworkDescription}");
        logger.Info($"Environment: {RuntimeInfo.OS} ({RuntimeInformation.OSDescription}), {Environment.ProcessorCount} cores");
        logger.Info(new string('-', 60));
        logger.Info(@"Surface:");
        logger.Info($"    System: {surface.Name} {surface.Version}");
        logger.Info($"      Kind: {(surface as INativeWindowSource)?.Native.Kind.ToString() ?? "unknown"}");

        int index = 0;
        foreach (var system in input.Systems)
        {
            logger.Info($"Input #{index}:");
            logger.Info($"    System: {system.Name} {system.Version}");
            index++;
        }

        logger.Info(@"Audio:");
        logger.Info($"    System: {audio.System} {audio.Version}");
        logger.Info($"    Device: {audio.Device}");
        logger.Info($"Extensions: {string.Join(' ', audio.Extensions)}");
        logger.Info(@"Graphics:");
        logger.Info($"    System: {graphics.System} {graphics.Version}");
        logger.Info($"    Device: {graphics.Device}");
        logger.Info($"Extensions: {string.Join(' ', graphics.Extensions)}");

        onInitAction?.Invoke(this);
        surface.Run();
        onExitAction?.Invoke(this);
    }

    private void onUpdateInit()
    {
        graphics.MakeCurrent();
        game = creator();
        game.Load();
    }

    private void onUpdateLoop()
    {
        time.Update();
        input.Update();
        game!.Update();
        graphics.Prepare(surface.Size);
        game!.Render();
        graphics.Present();

        if (!hasShownWindow && surface is IWindow window)
        {
            window.Visible = true;
            hasShownWindow = true;
        }
    }

    private void onUpdateExit()
    {
        game!.Dispose();
    }

    private void doWork()
    {
        ensureExecutionMode();

        switch (activeExecutionMode!.Value)
        {
            case ExecutionMode.SingleThread:
                {
                    lock (workers)
                    {
                        foreach (var t in workers)
                            t.DoWork();
                    }

                    break;
                }

            case ExecutionMode.MultiThread:
                Threads.Main.DoWork();
                break;
        }
    }

    private void ensureExecutionMode()
    {
        lock (syncLock)
        {
            var executionMode = this.executionMode;

            if (executionMode == activeExecutionMode)
                return;

            activeExecutionMode = executionMode;

            logger.Log("Execution Mode changed: {0}", executionMode);
        }

        switch (activeExecutionMode)
        {
            case ExecutionMode.MultiThread:
                {
                    foreach (var t in workers)
                        t.Start();

                    break;
                }

            case ExecutionMode.SingleThread:
                {
                    foreach (var t in workers)
                        t.Initialize(t == Threads.Main);

                    break;
                }
        }

        updateRates();
    }

    private void onSurfaceStateChange(bool state)
    {
        lock (syncLock)
        {
            if (state)
            {
                ensureExecutionMode();
            }
            else
            {
                foreach (var t in workers)
                    t.Pause();

                activeExecutionMode = null;
            }
        }
    }

    private void updateRates()
    {
        foreach (var worker in workers)
        {
            if (worker == Threads.Main)
            {
                worker.UpdatePerSecond = activeExecutionMode == ExecutionMode.SingleThread ? updatePerSecond : WorkerThread.DEFAULT_UPDATE_PER_SECOND;
            }
            else
            {
                worker.UpdatePerSecond = updatePerSecond;
            }
        }

        logger.Log("Update Rate changed: {0}", updatePerSecond);
    }

    private void handleUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        logger.Error(@"An unhandled exception has occured", exception);
        abortExecutionFromException(sender, exception, args.IsTerminating);
    }

    private void handleUnobservedException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        logger.Error(@"An unobserved exception has occured.", args.Exception);
        abortExecutionFromException(sender, args.Exception, false);
    }

    private void abortExecutionFromException(object? sender, Exception exception, bool isTerminating)
    {
        if (onException is not null && onException(exception))
            return;

        AppDomain.CurrentDomain.UnhandledException -= handleUnhandledException;
        TaskScheduler.UnobservedTaskException -= handleUnobservedException;

        var info = ExceptionDispatchInfo.Capture(exception);
        var mres = new ManualResetEventSlim();

        Threads.Main.Post(() =>
        {
            try
            {
                info.Throw();
            }
            finally
            {
                mres.Set();
            }
        });

        wait();

        void wait()
        {
            if (isTerminating || (sender is WorkerThread && sender == Threads.Main) || executionMode == ExecutionMode.SingleThread)
                return;

            mres.Wait(TimeSpan.FromSeconds(10));
        }
    }

    private void onSurfaceClose()
    {
        lock (workers)
        {
            foreach (var t in workers)
                t.Dispose();
        }

        current = null;
    }

    event Func<Exception, bool>? IHost.OnException
    {
        add => onException += value;
        remove => onException -= value;
    }

    ExecutionMode IHost.ExecutionMode
    {
        get => executionMode;
        set => executionMode = value;
    }

    double IHost.UpdatePerSecond
    {
        get => updatePerSecond;
        set
        {
            if (updatePerSecond == value)
                return;

            updatePerSecond = value;
            updateRates();
        }
    }

    string[] IHost.Arguments { get; } = Environment.GetCommandLineArgs();

    IDictionary IHost.Variables { get; } = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Process);

    ILogger IHost.Logger => logger;

    Surface IHost.Surface => surface;

    Storage IHost.Storage => storage;

    AudioContext IHost.Audio => audio;

    InputContext IHost.Input => input;

    GraphicsContext IHost.Graphics => graphics;

    void IHost.Exit()
    {
        surface.Close();
    }

    void IHostStartup.Run()
    {
        run();
    }

    Task IHostStartup.RunAsync()
    {
        return Task.Factory.StartNew(run, default, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    /// <summary>
    /// The host-owned threads.
    /// </summary>
    public class HostThreads
    {
        /// <summary>
        /// The main thread. This is the thread where <see cref="IHostStartup.Run"/> was called on.
        /// </summary>
        public readonly WorkerThread Main;

        /// <summary>
        /// The game thread. This is where many game-related functions are called on.
        /// </summary>
        public readonly WorkerThread Game;

        /// <summary>
        /// The audio thread. This is where all audio-related functions are called on.
        /// </summary>
        public readonly WorkerThread Audio;

        internal HostThreads(WorkerThread main, WorkerThread game, WorkerThread audio)
        {
            Main = main;
            Game = game;
            Audio = audio;
        }
    }
}
