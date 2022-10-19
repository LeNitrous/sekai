// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics;

namespace Sekai.Engine.Threading;

public abstract class UpdateThread : GameThread
{
    private readonly Stopwatch stopwatch = new();

    protected UpdateThread(string name = "unknown")
        : base($"Update ({name})")
    {
    }

    protected abstract void Update(double delta);

    protected sealed override void OnNewFrame()
    {
        stopwatch.Restart();
        Update(stopwatch.Elapsed.TotalMilliseconds);
    }
}
