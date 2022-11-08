// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

    private CancellationTokenSource? cts;
    private readonly WindowThread window;
    private readonly UpdateThread update;
    private readonly RenderThread render;
    private readonly Queue<Action> queue = new();
    private readonly List<UpdateThread> updates = new();

    internal ThreadController(WindowThread window, UpdateThread update, RenderThread render)
    {
        this.window = window;
        this.update = update;
        this.render = render;
        updates.Add(update);
    }

    /// <summary>
    /// Adds an update thread to this controller.
    /// </summary>
    public void Add(UpdateThread thread)
    {
        if (!updates.Contains(thread))
            updates.Add(thread);
    }

    /// <summary>
    /// Removes an update thread from this controller.
    /// </summary>
    public void Remove(UpdateThread thread)
    {
        if (thread == update)
            throw new InvalidOperationException(@"Cannot remove the main update thread.");

        updates.Remove(thread);
    }

    /// <summary>
    /// Dispatches an action to the main thread.
    /// </summary>
    public void Dispatch(Action action)
    {
        queue.Enqueue(action);
    }

    private readonly Stopwatch stopwatch = new();
    private ExecutionMode? mode;
    private bool firstTickRan;
    private double msPerUpdate => 1000d / UpdatePerSecond;
    private double previous;
    private double current;
    private double elapsed;
    private double lag;

    /// <summary>
    /// Starts the game loop.
    /// </summary>
    internal void Run()
    {
        if (IsRunning)
            return;

        IsRunning = true;

        cts = new CancellationTokenSource();

        while (!cts.Token.IsCancellationRequested)
        {
            RunSingleFrame(cts.Token);
        }
    }

    /// <summary>
    /// Runs a single frame of the game loop.
    /// </summary>
    internal void RunSingleFrame(CancellationToken token = default)
    {
        if (token.IsCancellationRequested)
            return;

        if (!firstTickRan)
        {
            stopwatch.Restart();
            firstTickRan = true;
        }

        if (!mode.HasValue || mode.Value != ExecutionMode)
        {
            lag = 0;
            mode = ExecutionMode;
            Logger.Log($"Execution mode changed to {ExecutionMode}.");
        }

        current = stopwatch.Elapsed.TotalMilliseconds;
        elapsed = current - previous;
        previous = current;

        lag += elapsed;

        while (queue.TryDequeue(out var action))
        {
            if (token.IsCancellationRequested)
                break;

            action.Invoke();
        }

        window.Process();

        if (ExecutionMode == ExecutionMode.SingleThread)
        {
            performFixedUpdate();

            for (int i = 0; i < updates.Count; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                updates[i].Update(elapsed);
            }

            render.Render();
        }

        if (ExecutionMode == ExecutionMode.MultiThread)
        {
            var updateFixed = Task.Factory.StartNew(performFixedUpdate, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            var updateTasks = new Task[updates.Count];

            for (int i = 0; i < updates.Count; i++)
            {
                if (token.IsCancellationRequested)
                    break;

                if (updates[i] is null)
                    continue;

                var thread = updates[i];
                updateTasks[i] = Task.Factory.StartNew(() => thread.Update(elapsed), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }

            try
            {
                updateFixed.Wait(token);
                Task.WaitAll(updateTasks, token);
            }
            catch (OperationCanceledException)
            {
            }

            render.Render();
        }

        OnTick?.Invoke();

        void performFixedUpdate()
        {
            while (lag >= msPerUpdate)
            {
                if (cts != null && cts.Token.IsCancellationRequested)
                    break;

                for (int i = 0; i < updates.Count; i++)
                {
                    if (cts != null && cts.Token.IsCancellationRequested)
                        break;

                    updates[i].FixedUpdate();
                }

                lag -= msPerUpdate;
            }
        }
    }

    /// <summary>
    /// Resets the state of the game loop.
    /// </summary>
    internal void Reset()
    {
        stopwatch.Stop();
        lag = 0;
        mode = null;
        current = 0;
        elapsed = 0;
        previous = 0;
        firstTickRan = false;
    }

    /// <summary>
    /// Stops the game loop.
    /// </summary>
    internal void Stop()
    {
        Dispose();
    }

    protected override void Destroy()
    {
        if (!IsRunning)
            return;

        cts.Cancel();

        if (!cts.Token.WaitHandle.WaitOne(10000))
            throw new TimeoutException("Failed to cancel in time.");

        Reset();

        IsRunning = false;
    }
}
