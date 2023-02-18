// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Sekai.Threading;

internal class WorkerThreadSynchronizationContext : SynchronizationContext
{
    private readonly WorkerThread worker;
    private readonly ConcurrentQueue<WorkItem> queue = new();

    internal WorkerThreadSynchronizationContext(WorkerThread worker)
    {
        this.worker = worker;
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        WorkItem work;

        queue.Enqueue(work = new WorkItem(d, state));

        while (!work.IsCompleted)
        {
            if (worker.IsCurrent)
            {
                DoWork();
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        queue.Enqueue(new(d, state));
    }

    public void DoWork()
    {
        while (queue.TryDequeue(out var item))
            item.Run();
    }

    private class WorkItem
    {
        public bool IsCompleted { get; private set; }

        private readonly object? state;
        private readonly SendOrPostCallback callback;

        public WorkItem(SendOrPostCallback callback, object? state)
        {
            this.state = state;
            this.callback = callback;
        }

        public void Run()
        {
            ExceptionDispatchInfo? info = null;

            try
            {
                callback(state);
            }
            catch (Exception ex)
            {
                info = ExceptionDispatchInfo.Capture(ex);
            }
            finally
            {
                IsCompleted = true;
            }

            info?.Throw();
        }
    }
}
