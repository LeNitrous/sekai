// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Threading;

namespace Sekai.Engine.Threading;

public abstract class RenderThread : FrameworkThread
{
    public RenderThread(string name = "unknown")
        : base($"Render ({name})")
    {
    }

    protected abstract void OnRenderFrame();

    protected override void OnNewFrame()
    {
        OnRenderFrame();
    }
}
