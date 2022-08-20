// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework;
using Sekai.Framework.Logging;
using Sekai.Framework.Threading;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Threading;

public sealed class ThreadController : FrameworkObject
{
    /// <summary>
    /// Whether this thread controller is currently running.
    /// </summary>
    public bool IsRunning => mainThread.IsRunning;

    /// <summary>
    /// Whether this thread controller should stop when an unhandled exception occurs.
    /// </summary>
    public bool AbortOnUnhandledException = true;

    /// <summary>
    /// Whether this thread controller should stop when an unobserved exception occurs.
    /// </summary>
    public bool AbortOnUnobservedException = false;

    /// <summary>
    /// Called when a thread has been added.
    /// </summary>
    public event Action<FrameworkThread> OnThreadAdded = null!;

    /// <summary>
    /// Called when a thread has been removed.
    /// </summary>
    public event Action<FrameworkThread> OnThreadRemoved = null!;

    private double updatePerSecond = 60;
    private double framesPerSecond = 60;
    private ExecutionMode executionMode;
    private bool executionModeChanged = true;
    private readonly IView view;
    private readonly FrameworkThread mainThread;
    private readonly List<FrameworkThread> threads = new();

    /// <summary>
    /// Gets or sets how threads will be ran.
    /// </summary>
    public ExecutionMode ExecutionMode
    {
        get => executionMode;
        set
        {
            if (executionMode == value)
                return;

            executionMode = value;
            executionModeChanged = true;
        }
    }

    /// <summary>
    /// Gets or sets how often the main thread and update threads will run.
    /// </summary>
    public double UpdatePerSecond
    {
        get => updatePerSecond;
        set
        {
            if (updatePerSecond == value)
                return;

            updatePerSecond = value;

            lock (threads)
            {
                foreach (var t in threads.OfType<UpdateThread>())
                    t.UpdatePerSecond = updatePerSecond;
            }

            mainThread.UpdatePerSecond = updatePerSecond;
        }
    }

    /// <summary>
    /// Gets or sets how often render threads will run.
    /// </summary>
    public double FramesPerSecond
    {
        get => framesPerSecond;
        set
        {
            if (framesPerSecond == value)
                return;

            lock (threads)
            {
                foreach (var t in threads.OfType<RenderThread>())
                    t.UpdatePerSecond = framesPerSecond;
            }

            framesPerSecond = value;
        }
    }

    public ThreadController(IView view)
    {
        this.view = view;
        mainThread = new MainThread(run);
        mainThread.OnUnhandledException += onUnhandledException;
        TaskScheduler.UnobservedTaskException += onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
    }

    /// <summary>
    /// Posts an action to the main thread.
    /// </summary>
    public void Post(Action action)
    {
        mainThread.Post(action);
    }

    /// <summary>
    /// Sends an action to the main thread.
    /// </summary>
    public void Send(Action action)
    {
        mainThread.Send(action);
    }

    /// <summary>
    /// Adds and starts thread to this manager.
    /// </summary>
    public void Add(FrameworkThread thread)
    {
        if (thread == mainThread)
            return;

        lock (threads)
        {
            if (threads.Contains(thread))
                return;

            threads.Add(thread);
        }

        thread.UpdatePerSecond = UpdatePerSecond;
        thread.OnUnhandledException += onUnhandledException;

        if (IsRunning && ExecutionMode == ExecutionMode.MultiThread)
            thread.Start();

        OnThreadAdded?.Invoke(thread);
    }

    /// <summary>
    /// Removes and stops a thread from this manager.
    /// </summary>
    public void Remove(FrameworkThread thread)
    {
        if (thread == mainThread)
            return;

        lock (threads)
        {
            if (!threads.Contains(thread))
                return;

            threads.Remove(thread);
        }

        thread.OnUnhandledException -= onUnhandledException;

        if (IsRunning && ExecutionMode == ExecutionMode.MultiThread)
            thread.Stop();

        OnThreadRemoved?.Invoke(thread);
    }

    /// <summary>
    /// Clears all threads from this manager.
    /// </summary>
    public void Clear()
    {
        foreach (var t in threads)
            Remove(t);
    }

    /// <summary>
    /// Starts the threading manager.
    /// </summary>
    internal void Run(Action? action = null)
    {
        if (IsRunning)
            return;

        if (action != null)
            Post(action);

        mainThread.Run();
    }

    /// <summary>
    /// Stops the threading manager.
    /// </summary>
    internal void Stop()
    {
        if (!IsRunning)
            return;

        mainThread.Stop();

        lock (threads)
        {
            foreach (var t in threads)
                t.Stop();
        }
    }

    private void onUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        Logger.Error(@"An unhandled exception has occured.", exception);

        if (AbortOnUnhandledException)
            abortFromException(sender, exception, args.IsTerminating);
    }

    private void onUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Logger.Error(@"An unobserved exception has occured.", args.Exception);

        if (AbortOnUnobservedException)
            abortFromException(sender, args.Exception, false);
    }

    private void abortFromException(object? sender, Exception exception, bool isTerminating)
    {
        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        using var reset = new ManualResetEventSlim(false);
        var exceptionInfo = ExceptionDispatchInfo.Capture(exception);

        mainThread.Post(() =>
        {
            try
            {
                exceptionInfo.Throw();
            }
            finally
            {
                reset.Set();
            }
        });

        if (isTerminating || sender == mainThread || ExecutionMode == ExecutionMode.SingleThread)
            return;

        reset.Wait(10000);
    }

    private void run()
    {
        ensureExecutionMode();

        view.DoEvents();

        if (ExecutionMode == ExecutionMode.SingleThread)
        {
            lock (threads)
            {
                foreach (var t in threads)
                    t.RunSingleFrame();
            }
        }
    }

    private void ensureExecutionMode()
    {
        if (!executionModeChanged)
            return;

        lock (threads)
        {
            foreach (var t in threads)
            {
                t.Stop();

                if (ExecutionMode == ExecutionMode.MultiThread)
                    t.Start();
            }
        }

        executionModeChanged = false;
    }

    protected override void Destroy()
    {
        Stop();

        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        mainThread.OnUnhandledException -= onUnhandledException;
        mainThread.Dispose();
        view.Dispose();

        lock (threads)
        {
            foreach (var t in threads)
                t.Dispose();
        }

        threads.Clear();
    }

    private class MainThread : FrameworkThread
    {
        protected override bool PropagateExceptions => true;

        private readonly Action onNewFrame;

        public MainThread(Action onNewFrame)
            : base(@"Main")
        {
            this.onNewFrame = onNewFrame;
        }

        protected override void OnNewFrame() => onNewFrame();
    }
}
