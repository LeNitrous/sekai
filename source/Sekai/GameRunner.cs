// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Allocation;
using Sekai.Logging;
using Sekai.Threading;

namespace Sekai;

public sealed class GameRunner : DependencyObject
{
    /// <summary>
    /// Invoked when an exception has been thrown. Return true to continue execution.
    /// </summary>
    public event Func<Exception, bool>? OnException;

    /// <inheritdoc cref="GameOptions.ExecutionMode"/>
    public ExecutionMode ExecutionMode { get; set; }

    /// <inheritdoc cref="GameOptions.UpdatePerSecond"/>
    public double UpdatePerSecond
    {
        get => updatePerSecond;
        set
        {
            updatePerSecond = value;
            updateMainThreadRates();
        }
    }

    [Resolved]
    private LoggerFactory loggerFactory { get; set; } = null!;

    private double updatePerSecond;
    private ExecutionMode? activeExecutionMode;
    private readonly Logger logger;
    private readonly WorkerThread main;
    private readonly object syncLock = new();
    private readonly List<WorkerThread> workers = new();
    private readonly Queue<Action> workerMutationQueue = new();

    internal GameRunner()
    {
        Add(main = new("Main", null, true));

        AppDomain.CurrentDomain.UnhandledException += handleUnhandledException;
        TaskScheduler.UnobservedTaskException += handleUnobservedException;

        logger = loggerFactory.GetLogger();
    }

    internal void Start()
    {
        ensureExecutionMode();
    }

    public void Add(WorkerThread thread)
    {
        workerMutationQueue.Enqueue(() =>
        {
            if (workers.Contains(thread))
                return;

            workers.Add(thread);
            thread.OnUnhandledException += handleUnhandledException;
        });
    }

    internal void Pause()
    {
        lock (syncLock)
        {
            pauseAllThreads();
            activeExecutionMode = null;
        }
    }

    internal void Update()
    {
        while (workerMutationQueue.TryDequeue(out var action))
            action();

        ensureExecutionMode();

        switch (activeExecutionMode.Value)
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
                main.DoWork();
                break;
        }
    }

    protected override void Destroy()
    {
        foreach (var t in workers)
            t.Dispose();
    }

    private void handleUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        logger.Error($"An unhandled error has occured.", exception);
        abortExecutionFromException(sender, exception, args.IsTerminating);
    }

    private void handleUnobservedException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        logger.Error($"An unobserved error has occured.", args.Exception);
        abortExecutionFromException(sender, args.Exception, false);
    }

    private void abortExecutionFromException(object? sender, Exception exception, bool isTerminating)
    {
        if (OnException is not null && OnException(exception))
            return;

        AppDomain.CurrentDomain.UnhandledException -= handleUnhandledException;
        TaskScheduler.UnobservedTaskException -= handleUnobservedException;

        var info = ExceptionDispatchInfo.Capture(exception);
        var mres = new ManualResetEventSlim();

        main.Post(() =>
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
            if (isTerminating || (sender is WorkerThread && sender == main) || ExecutionMode == ExecutionMode.SingleThread)
                return;

            mres.Wait(TimeSpan.FromSeconds(10));
        }
    }

    [MemberNotNull(nameof(activeExecutionMode))]
    private void ensureExecutionMode()
    {
        lock (syncLock)
        {
            var executionMode = ExecutionMode;

            if (executionMode == activeExecutionMode)
                return;

            logger.Info($"Execution mode changed: {activeExecutionMode} -> {executionMode}");
            activeExecutionMode = executionMode;
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
                        t.Initialize(t == main);

                    break;
                }
        }

        updateMainThreadRates();
    }

    private void pauseAllThreads()
    {
        foreach (var t in workers)
            t.Pause();
    }

    private void updateMainThreadRates() => main.UpdatePerSecond = activeExecutionMode == ExecutionMode.SingleThread ? updatePerSecond : WorkerThread.DEFAULT_UPDATE_PER_SECOND;
}
