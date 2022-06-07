// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;
using System.Threading;

namespace Sekai.Framework.Threading;

public class GameThread
{
    public readonly string Name;
    public event UnhandledExceptionEventHandler? OnUnhandledException;
    public event Action? OnNewFrame;
    public event Action? OnInitialize;
    public event Action? OnSuspend;
    public event Action? OnExit;
    public GameThreadState State { get; private set; }
    internal Thread? Thread { get; private set; }
    internal bool IsCurrent => Thread == null || Thread.CurrentThread == Thread;
    private bool throttled;
    private bool pauseRequested;
    private bool exitRequested;
    private double updatePeriod;
    private readonly GameThreadSynchronizationContext syncContext;
    private readonly object syncLock = new();
    private readonly Stopwatch stopwatch = new();

    internal double UpdatePerSecond
    {
        get => updatePeriod <= double.Epsilon ? 0 : 1 / updatePeriod;
        set => updatePeriod = value <= double.Epsilon ? 0 : 1 / value;
    }

    public GameThread(string name, Action? onNewFrame = null)
    {
        Name = name;
        OnNewFrame = onNewFrame;
        syncContext = new(this);
    }

    internal void Start()
    {
        lock (syncLock)
        {
            switch (State)
            {
                case GameThreadState.Idle:
                    break;
                case GameThreadState.Paused:
                    break;

                case GameThreadState.Running:
                case GameThreadState.Exited:
                default:
                    throw new InvalidOperationException($"Cannot start when thread is currently {State}.");
            }

            startDetached();
        }

        WaitForState(GameThreadState.Running);
    }

    internal void Pause()
    {
        lock (syncLock)
        {
            if (State != GameThreadState.Running)
                return;

            pauseRequested = true;
        }

        WaitForState(GameThreadState.Paused);
    }

    internal void Stop()
    {
        lock (syncLock)
        {
            switch (State)
            {
                case GameThreadState.Idle:
                case GameThreadState.Paused:
                default:
                    throw new InvalidCastException($"Cannot exit when thread is {State}.");

                case GameThreadState.Exited:
                    break;

                case GameThreadState.Running:
                    exitRequested = true;
                    break;
            }
        }
    }

    internal void Initialize(bool throttled)
    {
        lock (syncLock)
        {
            this.throttled = throttled;
            OnInitialize?.Invoke();
            stopwatch.Start();
            State = GameThreadState.Running;
        }
    }

    internal void MakeCurrent()
    {
        SynchronizationContext.SetSynchronizationContext(syncContext);
    }

    internal void RunSingleFrame()
    {
        var newState = processFrame();

        if (newState.HasValue)
            suspend(newState.Value);
    }

    internal void WaitForState(GameThreadState target)
    {
        if (State == target)
            return;

        if (Thread == null)
        {
            GameThreadState? state = null;

            while (state != target)
                state = processFrame();

            suspend(state.Value);
        }
        else
        {
            while (State != target)
                Thread.Sleep(1);
        }
    }

    private void startDetached()
    {
        Thread = new Thread(run)
        {
            Name = Name,
            IsBackground = true,
        };

        void run()
        {
            Initialize(true);

            while (State == GameThreadState.Running)
                RunSingleFrame();
        }

        Thread.Start();
    }

    private GameThreadState? processFrame()
    {
        if (State != GameThreadState.Running)
            return null;

        MakeCurrent();

        if (exitRequested)
        {
            exitRequested = false;
            return GameThreadState.Exited;
        }

        if (pauseRequested)
        {
            pauseRequested = false;
            return GameThreadState.Paused;
        }

        try
        {
            double start = stopwatch.Elapsed.TotalMilliseconds;

            OnNewFrame?.Invoke();
            syncContext.DoWork();

            double end = stopwatch.Elapsed.TotalMilliseconds;

            if (throttled)
                Thread.Sleep((int)Math.Max(0, start + (1000d / UpdatePerSecond) - end));
        }
        catch (Exception e)
        {
            OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(e, false));
        }

        return null;
    }

    private void suspend(GameThreadState state)
    {
        lock (syncLock)
        {
            Thread = null;
            OnSuspend?.Invoke();

            switch (state)
            {
                case GameThreadState.Idle:
                case GameThreadState.Running:
                default:
                    throw new InvalidOperationException($"Cannot suspend with {state}.");

                case GameThreadState.Paused:
                    break;

                case GameThreadState.Exited:
                    OnExit?.Invoke();
                    break;
            }

            State = state;
        }
    }
}
