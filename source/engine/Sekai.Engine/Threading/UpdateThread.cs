// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;
using Sekai.Framework.Threading;

namespace Sekai.Engine.Threading;

public abstract class UpdateThread : FrameworkThread
{
    private readonly Stopwatch stopwatch = new();

    protected UpdateThread(string name = "unknown")
        : base($"Update ({name})")
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
