// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;

namespace Sekai.Platform;

internal sealed class WaitableObject : IWaitable
{
    public void Wait(TimeSpan time)
    {
        Thread.Sleep(time);
    }

    public void Dispose()
    {
    }
}
