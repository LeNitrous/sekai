// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Sekai.Framework.Threading;

internal class FrameworkSynchronizationContext : SynchronizationContext
{
    private readonly FrameworkThread thread;
    private readonly ConcurrentQueue<WorkItem> runQueue = new();

    public FrameworkSynchronizationContext(FrameworkThread thread)
    {
        this.thread = thread;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        runQueue.Enqueue(new WorkItem(d, state));
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        var item = new WorkItem(d, state);
        runQueue.Enqueue(item);

        while (!item.Completed)
        {
            if (thread.IsCurrent)
            {
                DoWork();
            }
            else
            {
                Thread.Sleep(1);
            }
        }
    }

    public void DoWork()
    {
        while (runQueue.TryDequeue(out var item))
        {
            item.Execute();

            if (item.Exception != null)
                ExceptionDispatchInfo.Capture(item.Exception).Throw();
        }
    }

    private class WorkItem
    {
        public Exception? Exception { get; private set; }
        public bool Completed { get; private set; }
        private readonly SendOrPostCallback callback;
        private readonly object? state;

        public WorkItem(SendOrPostCallback callback, object? state)
        {
            this.state = state;
            this.callback = callback;
        }

        public void Execute()
        {
            try
            {
                callback(state);
            }
            catch (Exception e)
            {
                Exception = e;
            }
            finally
            {
                Completed = true;
            }
        }
    }
}
