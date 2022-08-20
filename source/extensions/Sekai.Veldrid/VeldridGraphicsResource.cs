// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Veldrid;

namespace Sekai.Veldrid;

internal abstract class VeldridGraphicsResource<T> : FrameworkObject
    where T : DeviceResource
{
    internal readonly T Resource;

    public VeldridGraphicsResource(T resource)
    {
        Resource = resource;
    }

    protected sealed override void Destroy()
    {
        if (Resource is IDisposable disposable)
            disposable.Dispose();
    }
}
