// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework.Logging;

namespace Sekai.Framework.Threading;

public sealed class ThreadController : FrameworkObject
{
    /// <summary>
    /// Whether this thread controller is currently running.
    /// </summary>
    [MemberNotNullWhen(true, "cts")]
    public bool IsRunning { get; private set; }

    /// <summary>
    /// Determines how often updates should happen under <see cref="ExecutionMode.SingleThread"/> execution.
    /// </summary>
    public double UpdatePerSecond { get; set; }

    /// <summary>
    /// Determines the execution mode for the thread controller.
    /// </summary>
    public ExecutionMode ExecutionMode { get; set; }

    /// <summary>
    /// Called every tick.
    /// </summary>
    public event Action OnTick = null!;

    /// <summary>
    /// Called when an exception has been caught. Return false to terminate the process.
    /// </summary>
    public event Func<Exception, bool> OnException = null!;

    private CancellationTokenSource? cts;
    private readonly WindowThread window;
    private readonly Queue<Action> queue = new();
    private readonly List<RenderThread> render = new();
    private readonly List<UpdateThread> update = new();

    internal ThreadController(WindowThread window)
    {
        this.window = window;
        TaskScheduler.UnobservedTaskException += onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += onUnhandledException;
    }

    /// <summary>
    /// Adds an update thread to this controller.
    /// </summary>
    public void Add(UpdateThread thread)
    {
        if (!update.Contains(thread))
            update.Add(thread);
    }

    /// <summary>
    /// Adds a render thread to this controller.
    /// </summary>
    public void Add(RenderThread thread)
    {
        if (!render.Contains(thread))
            render.Add(thread);
    }

    /// <summary>
    /// Removes an update thread from this controller.
    /// </summary>
    public void Remove(UpdateThread thread)
    {
        update.Remove(thread);
    }

    /// <summary>
    /// Removes a render thread from this controller.
    /// </summary>
    public void Remove(RenderThread thread)
    {
        render.Remove(thread);
    }

    /// <summary>
    /// Dispatches an action to the main thread.
    /// </summary>
    public void Dispatch(Action action)
    {
        queue.Enqueue(action);
    }

    internal void Run()
    {
        if (IsRunning)
            return;

        IsRunning = true;

        cts = new CancellationTokenSource();

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        double msPerUpdate = 1000d / UpdatePerSecond;
        double previous = stopwatch.Elapsed.TotalMilliseconds;
        double current = 0;
        double elapsed = 0;
        double lag = 0;

        var mode = ExecutionMode - 1;

        while (!cts.Token.IsCancellationRequested)
        {
            if (mode != ExecutionMode)
            {
                lag = 0;
                mode = ExecutionMode;
                Logger.Log($"Execution mode changed to {ExecutionMode}");
            }

            current = stopwatch.Elapsed.TotalMilliseconds;
            elapsed = current - previous;
            previous = current;

            lag += elapsed;

            while (queue.TryDequeue(out var action))
            {
                if (cts.Token.IsCancellationRequested)
                    break;

                action.Invoke();
            }

            window.Process();

            if (ExecutionMode == ExecutionMode.SingleThread)
            {
                performFixedUpdate();

                for (int i = 0; i < update.Count; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                        break;

                    update[i].Update(elapsed);
                }

                for (int i = 0; i < render.Count; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                        break;

                    render[i].Render();
                }
            }

            if (ExecutionMode == ExecutionMode.MultiThread)
            {
                var updateFixed = new[]
                {
                    Task.Factory.StartNew(performFixedUpdate, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                };

                var updateTasks = new Task[update.Count];

                for (int i = 0; i < update.Count; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                        break;

                    if (update[i] is null)
                        continue;

                    var thread = update[i];
                    updateTasks[i] = Task.Factory.StartNew(() => thread.Update(elapsed), cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }

                var renderTasks = new Task[render.Count];

                for (int i = 0; i < render.Count; i++)
                {
                    if (cts.Token.IsCancellationRequested)
                        break;

                    if (render[i] is null)
                        continue;

                    renderTasks[i] = Task.Factory.StartNew(render[i].Render, cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                }

                Task.WaitAll(updateFixed, cts.Token);
                Task.WaitAll(updateTasks, cts.Token);
                Task.WaitAll(renderTasks, cts.Token);
            }

            OnTick?.Invoke();
        }

        void performFixedUpdate()
        {
            while (lag >= msPerUpdate)
            {
                if (cts != null && cts.Token.IsCancellationRequested)
                    break;

                for (int i = 0; i < update.Count; i++)
                {
                    if (cts != null && cts.Token.IsCancellationRequested)
                        break;

                    update[i].FixedUpdate();
                }

                lag -= msPerUpdate;
            }
        }
    }

    internal void Stop()
    {
        Dispose();
    }

    private void onUnhandledException(object? sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        Logger.Error(@"An unhandled exception has occured.", exception);
        abortFromException(sender, exception, args.IsTerminating);
    }

    private void onUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        Logger.Error(@"An unobserved exception has occured.", args.Exception);
        abortFromException(sender, args.Exception, false);
    }

    private void abortFromException(object? sender, Exception exception, bool isTerminating)
    {
        if (!(OnException?.Invoke(exception) ?? false))
            return;

        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        using var reset = new ManualResetEventSlim(false);
        var info = ExceptionDispatchInfo.Capture(exception);

        Dispatch(() =>
        {
            try
            {
                info.Throw();
            }
            finally
            {
                reset.Set();
            }
        });

        if (isTerminating || sender == window || ExecutionMode == ExecutionMode.SingleThread)
            return;

        reset.Wait(10000);
    }

    protected override void Destroy()
    {
        if (!IsRunning)
            return;

        TaskScheduler.UnobservedTaskException -= onUnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException -= onUnhandledException;

        cts.Cancel();

        if (!cts.Token.WaitHandle.WaitOne(10000))
            throw new TimeoutException("Failed to cancel in time.");

        IsRunning = false;
    }
}
