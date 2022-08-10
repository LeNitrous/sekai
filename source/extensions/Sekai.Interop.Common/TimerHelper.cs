// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

// Contains code from MonoGame.
// Copyright 2021 the MonoGame Team.
// Licensed under the Microsoft Public License and MIT.
// See https://github.com/MonoGame/MonoGame/blob/develop/LICENSE.txt.
using System.Runtime.InteropServices;

namespace Sekai.Interop.Common;
internal static class TimerHelper
{
    [DllImport("ntdll.dll", SetLastError = true)]
    private static extern int NtQueryTimerResolution(out uint minimumResolution, out uint maximumResolution, out uint currentResolution);

    private static readonly double lowestSleepThreshold;

    static TimerHelper()
    {
        NtQueryTimerResolution(out uint min, out uint max, out uint current);
        lowestSleepThreshold = 1.0 + (max / 10000.0);
    }

    /// <summary>
    /// Returns the current timer resolution in milliseconds
    /// </summary>
    public static double GetCurrentResolution()
    {
        NtQueryTimerResolution(out uint min, out uint max, out uint current);
        return current / 10000.0;
    }

    /// <summary>
    /// Sleeps as long as possible without exceeding the specified period
    /// </summary>
    public static void SleepForNoMoreThan(double milliseconds)
    {
        // Assumption is that Thread.Sleep(t) will sleep for at least (t), and at most (t + timerResolution)
        if (milliseconds < lowestSleepThreshold)
            return;
        int sleepTime = (int)(milliseconds - GetCurrentResolution());
        if (sleepTime > 0)
            Thread.Sleep(sleepTime);
    }
}
