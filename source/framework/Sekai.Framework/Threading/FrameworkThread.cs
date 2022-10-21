// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Threading;

public abstract class FrameworkThread : FrameworkObject
{
    /// <summary>
    /// The name for this thread.
    /// </summary>
    public readonly string Name;

    protected FrameworkThread(string name)
    {
        Name = name;
    }

    public abstract void Process();
}
