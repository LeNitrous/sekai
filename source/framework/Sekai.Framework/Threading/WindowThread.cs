// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Threading;

public abstract class WindowThread : FrameworkThread
{
    protected WindowThread()
        : base("Main")
    {
    }
}
