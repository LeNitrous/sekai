// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Sekai.Framework.Threading;

public abstract class FrameworkThread : FrameworkObject
{
    /// <summary>
    /// The name for this thread.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Whether the thread is currently running or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(cts))]
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Whether the thread is running in the background or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(thread))]
    public bool IsBackground => thread != null;

    /// <summary>
    /// Whether this is the current thread when called.
    /// </summary>
    public bool IsCurrent => thread == null && Thread.CurrentThread == thread;

    /// <summary>
    /// Raised when an unhandled exception has occured.
    /// </summary>
    public event UnhandledExceptionEventHandler? OnUnhandledException;

    /// <summary>
    /// Gets or sets how frequent this thread should update in seconds.
    /// </summary>
    internal double UpdatePerSecond { get; set; }

    /// <summary>
    /// Whether exceptions should be propagated or not.
    /// </summary>
    protected virtual bool PropagateExceptions => false;

    private Thread? thread;
    private CancellationTokenSource? cts;
    private readonly Stopwatch stopwatch = new();
    private readonly FrameworkSynchronizationContext syncContext;

    protected FrameworkThread(string name)
    {
        Name = name;
        syncContext = new(this);
        UpdatePerSecond = 60;
    }

    /// <summary>
    /// Posts a callback to the queue to be invoked by this thread.
    /// </summary>
    public void Post(Action action)
    {
        syncContext.Post(_ => action(), null);
    }

    /// <summary>
    /// Sends a callback to the queue to be invoked by this thread.
    /// </summary>
    public void Send(Action action)
    {
        syncContext.Send(_ => action(), null);
    }

    /// <summary>
    /// Runs this thread's loop on the calling thread.
    /// </summary>
    internal void Run()
    {
        if (IsRunning)
            return;

        IsRunning = true;

        cts = new CancellationTokenSource();
        stopwatch.Start();

        while (!cts.Token.IsCancellationRequested)
        {
            stopwatch.Restart();

            Perform();

            Thread.Sleep((int)Math.Max(0, (1000d / UpdatePerSecond) - stopwatch.Elapsed.TotalMilliseconds));
        }
    }

    /// <summary>
    /// Starts and runs a thread in the background.
    /// </summary>
    /// <remarks>
    /// The calling thread is blocked until this thread has fully started.
    /// </remarks>
    internal void Start()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(FrameworkThread));

        if (IsRunning)
            return;

        using var reset = new ManualResetEventSlim(false);

        thread = new Thread(run)
        {
            Name = Name,
            IsBackground = true,
        };

        thread.Start();
        reset.Wait();

        void run()
        {
            reset.Set();
            Run();
        }
    }

    /// <summary>
    /// Stops the running thread from running in the background.
    /// </summary>
    /// <remarks>
    /// The calling thread is blocked until this thread has fully stopped.
    /// </remarks>
    internal void Stop()
    {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(FrameworkThread));

        if (!IsRunning)
            return;

        cts.Cancel();

        if (IsBackground && !thread.Join(30000))
            throw new TimeoutException($"Thread took too long to stop.");

        IsRunning = false;

        thread = null;
        stopwatch.Reset();
    }

    /// <summary>
    /// Runs a single frame for this thread.
    /// </summary>
    internal void RunSingleFrame()
    {
        MakeCurrent();

        try
        {
            syncContext.DoWork();
            OnNewFrame();
        }
        catch (Exception e)
        {
            if (PropagateExceptions)
            {
                throw;
            }
            else
            {
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(e, false));
            }
        }
    }

    /// <summary>
    /// Sets the current synchronization context to the one owned by this thread.
    /// </summary>
    internal void MakeCurrent()
    {
        SynchronizationContext.SetSynchronizationContext(syncContext);
    }

    /// <summary>
    /// Called every frame.
    /// </summary>
    protected virtual void OnNewFrame()
    {
    }

    /// <summary>
    /// The action called in <see cref="Run"/>
    /// </summary>
    protected virtual void Perform() => RunSingleFrame();

    protected override void Destroy()
    {
        if (IsRunning)
            Stop();
    }
}
