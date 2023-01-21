// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics;

namespace Sekai;

public sealed class Time : FrameworkObject
{
    /// <summary>
    /// The number of frames since running the game.
    /// </summary>
    public int FrameCount { get; private set; }

    /// <summary>
    /// Time between the last frame and the current frame.
    /// </summary>
    public TimeSpan Delta { get; private set; }

    /// <summary>
    /// The total amount of time since running the game.
    /// </summary>
    public TimeSpan Current { get; private set; }

    private TimeSpan previous;
    private readonly Stopwatch stopwatch = new();

    internal void Update()
    {
        if (!stopwatch.IsRunning)
            stopwatch.Start();

        FrameCount++;
        previous = Current;
        Current = stopwatch.Elapsed;
        Delta = Current - previous;
    }

    internal void Reset()
    {
        Delta = TimeSpan.Zero;
        Current = TimeSpan.Zero;
        FrameCount = 0;

        previous = TimeSpan.Zero;
        stopwatch.Reset();
    }
}
