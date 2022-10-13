// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Threading;

public abstract class RenderThread : GameThread
{
    public RenderThread(string name = "unknown")
        : base($"Render ({name})")
    {
    }

    protected abstract void Render();

    protected override void OnNewFrame()
    {
        Render();
    }
}
