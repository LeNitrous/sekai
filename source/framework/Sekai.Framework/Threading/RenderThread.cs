// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Threading;

public abstract class RenderThread : FrameworkThread
{
    protected RenderThread(string name)
        : base($"Render ({name})")
    {
    }
}
