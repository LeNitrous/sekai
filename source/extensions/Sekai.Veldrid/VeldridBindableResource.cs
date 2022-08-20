// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Veldrid;

namespace Sekai.Veldrid;

internal class VeldridBindableResource<T> : VeldridGraphicsResource<T>
    where T : BindableResource, DeviceResource
{
    internal readonly T Bindable;

    public VeldridBindableResource(T resource)
        : base(resource)
    {
        Bindable = resource;
    }
}
