// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;

namespace Sekai.Platform;

public sealed class DefaultWaitableObjectFactory : IWaitableFactory
{
    public static readonly IWaitableFactory Instance = new DefaultWaitableObjectFactory();

    public IWaitable CreateWaitable() => DefaultWaitableObject.Instance;

    private DefaultWaitableObjectFactory()
    {
    }
}

internal sealed class DefaultWaitableObject : IWaitable
{
    public static IWaitable Instance = new DefaultWaitableObject();

    private DefaultWaitableObject()
    {
    }

    public void Wait(TimeSpan time)
    {
        Thread.Sleep(time);
    }

    public void Dispose()
    {
    }
}
