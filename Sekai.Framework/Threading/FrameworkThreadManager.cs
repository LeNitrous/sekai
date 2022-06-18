// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework.Logging;

namespace Sekai.Framework.Threading;

public abstract class FrameworkThreadManager : FrameworkObject
{
    /// <summary>
    /// Whether this thread manager is currently running.
    /// </summary>
    public bool IsRunning => mainThread.IsRunning;

    private double updatePerSecond = 60;
    private double framesPerSecond = 60;
    private ExecutionMode executionMode;
    private bool executionModeChanged = true;
    private readonly MainThread mainThread;
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

            framesPerSecond = value;

            lock (threads)
            {
                foreach (var t in threads.OfType<RenderThread>())
                    t.UpdatePerSecond = framesPerSecond;
            }
        }
    }

    internal FrameworkThreadManager()
    {
        mainThread = CreateMainThread();
        mainThread.OnPerform += run;
        mainThread.OnUnhandledException += onUnhandledException;
        TaskScheduler.UnobservedTaskException += onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
    }

    /// <summary>
    /// Posts a callback to the main thread's queue.
    /// </summary>
    public void Post(Action action)
    {
        mainThread.Post(action);
    }

    /// <summary>
    /// Sends a callback to the main thread's queue.
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

        thread.UpdatePerSecond = thread is RenderThread ? FramesPerSecond : UpdatePerSecond;
        thread.OnUnhandledException += onUnhandledException;

        if (IsRunning && ExecutionMode == ExecutionMode.MultiThread)
            thread.Start();

        OnThreadAdded(thread);
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

        OnThreadRemoved(thread);
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
    internal void Run()
    {
        if (IsRunning)
            return;

        Initialize();

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

    /// <summary>
    /// Creates the main thread for this manager.
    /// </summary>
    protected abstract MainThread CreateMainThread();

    /// <summary>
    /// Prepares this thread manager for running.
    /// </summary>
    protected virtual void Initialize()
    {
    }

    /// <summary>
    /// Called when a thread has been added and started.
    /// </summary>
    protected virtual void OnThreadAdded(FrameworkThread thread)
    {
    }

    /// <summary>
    /// Called when a thread is being removed and stopped.
    /// </summary>
    protected virtual void OnThreadRemoved(FrameworkThread thread)
    {
    }

    /// <summary>
    /// Called when an unhandled exception occurs. Return true to abort the process.
    /// </summary>
    protected virtual bool OnUnhandledException(Exception exception) => true;

    /// <summary>
    /// Called when an unobserved task exception occurs. Return true to abort the process.
    /// </summary>
    protected virtual bool OnUnobservedException(Exception exception) => false;

    private void onUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        Logger.Error(@"An unhandled exception has occured.", exception);

        if (OnUnhandledException(exception))
            abortFromException(sender, exception, args.IsTerminating);
    }

    private void onUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Logger.Error(@"An unobserved exception has occured.", args.Exception);

        if (OnUnobservedException(args.Exception))
            abortFromException(sender, args.Exception, false);
    }

    private void abortFromException(object? sender, Exception exception, bool isTerminating)
    {
        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        using var reset = new ManualResetEventSlim(false);
        var exceptionInfo = ExceptionDispatchInfo.Capture(exception);

        Post(() =>
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

        if (isTerminating || (sender is FrameworkThread && (sender == mainThread || ExecutionMode == ExecutionMode.SingleThread)))
            return;

        reset.Wait(10000);
    }

    private void run()
    {
        ensureExecutionMode();

        mainThread.RunSingleFrame();

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

        mainThread.OnPerform -= run;
        mainThread.OnUnhandledException -= onUnhandledException;
        mainThread.Dispose();

        lock (threads)
        {
            foreach (var t in threads)
                t.Dispose();
        }

        threads.Clear();
    }
}
