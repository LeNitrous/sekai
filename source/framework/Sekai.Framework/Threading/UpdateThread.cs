// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Framework.Threading;

public abstract class UpdateThread : FrameworkThread
{
    private readonly Stopwatch stopwatch = new();

    protected UpdateThread(string name = "unknown")
        : base($"Update ({name})")
    {
    }

    internal UpdateThread()
        : this("Main")
    {
    }

    protected abstract void OnUpdateFrame(double delta);

    protected sealed override void OnNewFrame()
    {
        stopwatch.Restart();
        OnUpdateFrame(stopwatch.Elapsed.TotalMilliseconds);
    }

    protected override void Destroy() => stopwatch.Reset();
}
