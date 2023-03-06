// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;

namespace Sekai.Platform;

/// <summary>
/// An <see cref="IWaitable"/> that uses <seealso cref="Thread.Sleep(TimeSpan)"/>.
/// </summary>
internal class DefaultWaitableObject : IWaitable
{
    public void Wait(TimeSpan time)
    {
        Thread.Sleep(time);
    }
}
