// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sekai.Threading;

public partial class WorkerThread : FrameworkObject
{
    /// <summary>
    /// The thread name.
    /// </summary>
    public readonly string Name;

    /// <summary>
    /// Gets or sets the update rate for this thread.
    /// </summary>
    public double UpdatePerSecond { get; set; } = DEFAULT_UPDATE_PER_SECOND;

    /// <summary>
    /// Whether this thread is currently running.
    /// </summary>
    public bool IsActive => currentState == WorkerThreadState.Active;

    /// <summary>
    /// Whether this thread is currently paused.
    /// </summary>
    public bool IsPaused => currentState == WorkerThreadState.Paused;

    /// <summary>
    /// Whether this thread is the current thread.
    /// </summary>
    public bool IsCurrent => thread is not null && Thread.CurrentThread == thread;

    /// <summary>
    /// The thread synchronization context.
    /// </summary>
    public SynchronizationContext SyncContext => syncContext;

    /// <summary>
    /// Invoked when on a new frame.
    /// </summary>
    public event Action? OnNewFrame;

    /// <summary>
    /// Invoked when the thread is starting.
    /// </summary>
    public event Action? OnStart;

    /// <summary>
    /// Invoked when the thread is paused.
    /// </summary>
    public event Action? OnPause;

    /// <summary>
    /// Invoked when the thread has exited.
    /// </summary>
    public event Action? OnExit;

    /// <summary>
    /// Invoked when an exception is thrown.
    /// </summary>
    public event UnhandledExceptionEventHandler? OnUnhandledException;

    internal const double DEFAULT_UPDATE_PER_SECOND = 60;

    private Thread? thread;
    private bool throttling;
    private WorkerThreadState currentState;
    private readonly bool isMainThread;
    private volatile bool exitRequested;
    private volatile bool pauseRequested;
    private readonly object syncLock = new();
    private readonly Stopwatch stopwatch = new();
    private readonly WorkerThreadSynchronizationContext syncContext = new();

    public WorkerThread(string name = "Worker Thread", Action? onNewFrame = null)
        : this(name, onNewFrame, false)
    {
    }

    internal WorkerThread(string name = "Worker Thread", Action? onNewFrame = null, bool isMainThread = false)
    {
        Name = name;
        OnNewFrame += onNewFrame;

        if (RuntimeInfo.OS == RuntimeInfo.Platform.Windows)
            createWaitableTimer();

        this.isMainThread = isMainThread;
    }

    /// <summary>
    /// Sends a synchronous action to this thread. The thread is blocked until the action is performed.
    /// </summary>
    public void Send(Action action) => syncContext.Send(_ => action(), null);

    /// <summary>
    /// Posts an asynchronous action to this thread. The thread will not be blocked.
    /// </summary>
    public void Post(Action action) => syncContext.Post(_ => action(), null);

    internal void Start()
    {
        if (IsActive)
            return;

        thread = new Thread(runWork)
        {
            Name = Name,
            IsBackground = true
        };

        thread.Start();
        WaitForState(WorkerThreadState.Active);

        void runWork()
        {
            Initialize();

            while (IsActive)
                DoWork();
        }
    }

    internal void Pause()
    {
        lock (syncLock)
        {
            if (!IsPaused)
                return;

            pauseRequested = true;
        }

        WaitForState(WorkerThreadState.Paused);
    }

    internal void Initialize(bool throttled = true)
    {
        MakeCurrent();
        throttling = throttled;
        currentState = WorkerThreadState.Active;
        stopwatch.Start();
        OnStart?.Invoke();
    }

    internal void WaitForState(WorkerThreadState state)
    {
        if (currentState == state)
            return;

        if (thread == null)
        {
            WorkerThreadState? next = null;

            while (next != state)
                next = processFrame();

            handleStateChangeFromActive(next.Value);
        }
        else
        {
            while (currentState != state)
                Thread.Sleep(1);
        }
    }

    internal void MakeCurrent() => SynchronizationContext.SetSynchronizationContext(syncContext);

    internal void DoWork()
    {
        var next = processFrame();

        if (next.HasValue)
            handleStateChangeFromActive(next.Value);
    }

    protected sealed override void Destroy()
    {
        lock (syncLock)
        {
            if (!IsActive)
                return;

            exitRequested = true;
        }

        if (thread != null)
        {
            if (!thread.Join(30000))
                throw new TimeoutException($"Thread {Name} failed to exit in time.");
        }
        else
        {
            WaitForState(WorkerThreadState.Exited);
        }

        if (highResolutionTimer != IntPtr.Zero)
            CloseHandle(highResolutionTimer);
    }

    private WorkerThreadState? processFrame()
    {
        if (currentState != WorkerThreadState.Active)
            return null;

        MakeCurrent();

        if (exitRequested)
        {
            exitRequested = false;
            return WorkerThreadState.Exited;
        }

        if (pauseRequested)
        {
            pauseRequested = false;
            return WorkerThreadState.Paused;
        }

        double start = stopwatch.ElapsedMilliseconds;
        double frame = 1 / UpdatePerSecond * 1000;

        try
        {
            syncContext.DoWork();
            OnNewFrame?.Invoke();

            if (throttling)
            {
                double duration = stopwatch.ElapsedMilliseconds - start;

                if (frame > duration)
                    sleep(TimeSpan.FromMilliseconds(frame - duration));
            }
        }
        catch (Exception e)
        {
            if (!isMainThread)
            {
                OnUnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(e, false));
            }
            else
            {
                throw;
            }
        }

        return null;
    }

    private void handleStateChangeFromActive(WorkerThreadState state)
    {
        lock (syncLock)
        {
            if (currentState != WorkerThreadState.Active)
                throw new InvalidOperationException(@"Cannot be invoked when current state is not currently active.");

            thread = null;
            OnPause?.Invoke();

            if (state == WorkerThreadState.Exited)
            {
                OnExit?.Invoke();
                stopwatch.Stop();
            }

            currentState = state;
        }
    }

    private nint highResolutionTimer;

    private void sleep(TimeSpan ts)
    {
        bool slept = false;

        if (highResolutionTimer != IntPtr.Zero)
        {
            ulong time = unchecked((ulong)-ts.Ticks);

            var fileTime = new FILETIME
            {
                DateTimeLow = (uint)(time & 0xFFFFFFFF),
                DateTimeHigh = (uint)(time >> 32),
            };

            if (SetWaitableTimerEx(highResolutionTimer, fileTime, 0, null!, default, IntPtr.Zero, 0))
            {
                if (WaitForSingleObject(highResolutionTimer, timer_infinite) != 0x00000000)
                    throw new Win32Exception();

                slept = true;
            }
        }

        if (!slept)
            Thread.Sleep(ts);
    }

    private void createWaitableTimer()
    {
        try
        {
            highResolutionTimer = CreateWaitableTimerEx(IntPtr.Zero, null, CreateWaitableTimerFlags.CREATE_WAITABLE_TIMER_MANUAL_RESET | CreateWaitableTimerFlags.CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, timer_all_access);

            if (highResolutionTimer == IntPtr.Zero)
            {
                CreateWaitableTimerEx(IntPtr.Zero, null!, CreateWaitableTimerFlags.CREATE_WAITABLE_TIMER_MANUAL_RESET, timer_all_access);
            }
        }
        catch
        {
        }
    }

    private const uint timer_infinite = 0xffffffff;
    private const uint timer_all_access = 2031619U;

    [LibraryImport("kernel32.dll", EntryPoint = "CreateWaitableTimerExW", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint CreateWaitableTimerEx(nint lpTimerAttributes, string? lpTimerName, CreateWaitableTimerFlags dwFlags, uint dwDesiredAccess);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWaitableTimerEx(nint hTimer, in FILETIME lpDueTime, int lPeriod, TimerAPCProc routine, nint lpArgToCompletionRoutine, nint reason, uint tolerableDelay);

    [LibraryImport("kernel32.dll")]
    private static partial int WaitForSingleObject(nint handle, uint milliseconds);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint hObject);

    private delegate void TimerAPCProc(nint completionArg, uint timerLowValue, uint timerHighValue);

    [Flags]
    internal enum CreateWaitableTimerFlags : uint
    {
        CREATE_WAITABLE_TIMER_MANUAL_RESET = 0x00000001,
        CREATE_WAITABLE_TIMER_HIGH_RESOLUTION = 0x00000002,
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct FILETIME
    {
        public uint DateTimeLow;
        public uint DateTimeHigh;
    }
}
