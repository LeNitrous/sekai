// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.System;

public abstract class FrameworkObject : IFrameworkObject
{
    public bool IsDisposed { get; private set; }

    protected virtual void Destroy()
    {
    }

    public void Dispose()
    {
        if (IsDisposed)
            return;

        Destroy();

        IsDisposed = true;
        GC.SuppressFinalize(this);
    }
}
