// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Threading;

public abstract class UpdateThread : FrameworkThread
{
    protected UpdateThread(string name)
        : base($"Update ({name})")
    {
    }
}
