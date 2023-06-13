// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Sekai.Platform;

internal partial struct Waiter
{
    private nint highResolutionTimer;

    public Waiter()
    {
        try
        {
            highResolutionTimer = CreateWaitableTimerEx(IntPtr.Zero, null, CreateWaitableTimerFlags.CREATE_WAITABLE_TIMER_MANUAL_RESET | CreateWaitableTimerFlags.CREATE_WAITABLE_TIMER_HIGH_RESOLUTION, timer_all_access);

            if (highResolutionTimer == IntPtr.Zero)
            {
                highResolutionTimer = CreateWaitableTimerEx(IntPtr.Zero, null, CreateWaitableTimerFlags.CREATE_WAITABLE_TIMER_MANUAL_RESET, timer_all_access);
            }
        }
        catch
        {
        }
    }

    public void Wait(TimeSpan time)
    {
        bool slept = false;

        if (highResolutionTimer != IntPtr.Zero)
        {
            ulong ticks = unchecked((ulong)-time.Ticks);

            var fileTime = new FILETIME
            {
                DateTimeL = (uint)(ticks & 0xFFFFFFFF),
                DateTimeH = (uint)(ticks >> 32)
            };

            if (SetWaitableTimerEx(highResolutionTimer, fileTime, 0, null!, IntPtr.Zero, IntPtr.Zero, 0))
            {
                if (WaitForSingleObject(highResolutionTimer, timer_infinite) != 0)
                {
                    throw new Win32Exception();
                }

                slept = true;
            }
        }

        if (!slept)
        {
            // Fallback to System.Threading.Thread when high resolution timers are unavailable.
            Thread.Sleep(time);
        }
    }

    public void Dispose()
    {
        if (highResolutionTimer == IntPtr.Zero)
        {
            return;
        }

        CloseHandle(highResolutionTimer);

        highResolutionTimer = IntPtr.Zero;
    }

    [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Utf8)]
    private static partial nint CreateWaitableTimerEx(nint lpTimerAttributes, string? lpTimerName, CreateWaitableTimerFlags dwFlags, uint dwDesiredAccess);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetWaitableTimerEx(nint hTimer, in FILETIME lpDueTime, int lPeriod, TimerAPCProc routine, nint lpArgToCompletionRoutine, nint reason, uint tolerableDelay);

    [LibraryImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseHandle(nint hObject);

    [LibraryImport("kernel32.dll")]
    private static partial int WaitForSingleObject(nint handle, uint milliseconds);

    private const uint timer_infinite = 0xffffffff;
    private const uint timer_all_access = 2031619U;

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    private delegate void TimerAPCProc(nint completionArg, uint timerLowValue, uint timerHighValue);

    [StructLayout(LayoutKind.Sequential)]
    private struct FILETIME
    {
        public uint DateTimeL;
        public uint DateTimeH;
    }

#pragma warning disable RCS1135

    [Flags]
    private enum CreateWaitableTimerFlags : uint
    {
        CREATE_WAITABLE_TIMER_MANUAL_RESET = 0x00000001,
        CREATE_WAITABLE_TIMER_HIGH_RESOLUTION = 0x00000002,
    }

#pragma warning restore RCS1135
}
