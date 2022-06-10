// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sekai.Framework.Logging;

namespace Sekai.Framework.Threading;

public class GameThreadManager : FrameworkObject, IReadOnlyList<GameThread>
{
    private readonly List<GameThread> threads = new();
    private IEnumerable<GameThread> updateThreads => threads.Except(renderThreads);
    private IEnumerable<GameThread> renderThreads => threads.OfType<RenderThread>();
    private CancellationTokenSource? cancellationTokenSource;
    private GameThread? mainThread;
    private double updatePerSecond;
    private double framesPerSecond;
    private bool running;
    private bool executionModeChanged = true;
    private ExecutionMode executionMode;

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

    public double UpdatePerSecond
    {
        get => updatePerSecond;
        set
        {
            if (updatePerSecond == value)
                return;

            updatePerSecond = value;

            foreach (var t in updateThreads)
                t.UpdatePerSecond = updatePerSecond;
        }
    }

    public double FramesPerSecond
    {
        get => framesPerSecond;
        set
        {
            if (framesPerSecond == value)
                return;

            framesPerSecond = value;

            foreach (var t in renderThreads)
                t.UpdatePerSecond = framesPerSecond;
        }
    }

    public int Count => threads.Count;
    public GameThread this[int index] => threads[index];

    internal GameThreadManager()
    {
        AppDomain.CurrentDomain.UnhandledException += handleUnhandledException;
        TaskScheduler.UnobservedTaskException += handleUnobservedException;
    }

    public void Add(GameThread thread)
    {
        lock (threads)
        {
            if (!threads.Contains(thread))
            {
                threads.Add(thread);
                mainThread ??= thread;

                thread.OnUnhandledException += handleUnhandledException;
                thread.UpdatePerSecond = thread is RenderThread
                    ? FramesPerSecond
                    : UpdatePerSecond;

                if (running)
                {
                    startThread(thread);
                }
            }
        }
    }

    public void Remove(GameThread thread)
    {
        lock (threads)
        {
            if (thread == mainThread)
                throw new InvalidOperationException(@"Cannot unregister main thread.");

            if (threads.Contains(thread))
            {
                threads.Remove(thread);

                if (running)
                {
                    stopThread(thread);
                }

                thread.OnUnhandledException -= handleUnhandledException;
            }
        }
    }

    internal void Stop()
    {
        if (!running)
            return;

        running = false;

        foreach (var t in threads)
            stopThread(t);

        cancellationTokenSource?.Cancel();
    }

    internal void Suspend()
    {
        pauseAllThreads();
    }

    internal void Run()
    {
        if (running)
            return;

        running = true;
        cancellationTokenSource = new();

        while (true)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                cancellationTokenSource = null;
                break;
            }

            ensureExecutionMode();

            switch (ExecutionMode)
            {
                case ExecutionMode.MultiThread:
                    {
                        mainThread?.RunSingleFrame();
                        break;
                    }

                case ExecutionMode.SingleThread:
                    {
                        lock (threads)
                        {
                            foreach (var t in threads)
                                t.RunSingleFrame();
                        }
                        break;
                    }

                default:
                    throw new InvalidOperationException(@"Invalid execution mode.");
            }
        }
    }

    private void ensureExecutionMode()
    {
        if (!executionModeChanged)
            return;

        pauseAllThreads();

        foreach (var t in threads)
        {
            if (t == mainThread)
                continue;

            startThread(t);
        }

        mainThread?.Initialize(true);

        executionModeChanged = false;
    }

    private void pauseAllThreads()
    {
        foreach (var t in threads)
            t.Pause();
    }

    private void startThread(GameThread t)
    {
        switch (ExecutionMode)
        {
            case ExecutionMode.MultiThread:
                t.Start();
                break;

            case ExecutionMode.SingleThread:
                t.Initialize(false);
                break;

            default:
                throw new InvalidOperationException(@"Invalid execution mode.");
        }
    }

    private static void stopThread(GameThread t)
    {
        var thread = t.Thread;

        t.Stop();

        if (thread != null)
        {
            if (!thread.Join(30000))
                throw new TimeoutException($"Thread {t.Name} took to long to exit.");
        }
        else
        {
            t.WaitForState(GameThreadState.Exited);
        }
    }

    private void handleUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        var exception = (Exception)args.ExceptionObject;
        Logger.Error($"An unhandled exception has occured.", exception);
    }

    private void handleUnobservedException(object? sender, UnobservedTaskExceptionEventArgs args)
    {
        var exception = args.Exception.Flatten();
        Logger.Error($"An unobserved exception has occured.", exception);
    }

    protected override void Destroy()
    {
        Stop();
        AppDomain.CurrentDomain.UnhandledException -= handleUnhandledException;
        TaskScheduler.UnobservedTaskException -= handleUnobservedException;
    }

    public IEnumerator<GameThread> GetEnumerator()
    {
        return threads.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
