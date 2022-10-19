// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Rendering;

internal class RenderParameter : FrameworkObject
{
    public readonly string Name;
    public readonly IBindableResource Resource;

    public RenderParameter(string name, IBindableResource resource)
    {
        Name = name;
        Resource = resource;
    }

    protected override void Destroy()
    {
        Resource.Dispose();
    }
}

internal class RenderParameter<T> : RenderParameter
    where T : struct, IEquatable<T>
{
    public new IBuffer Resource => (IBuffer)base.Resource;

    public T Value
    {
        get => value;
        set
        {
            if (this.value.Equals(value))
                return;

            this.value = value;

            device.UpdateBufferData(Resource, ref this.value, 0);
        }
    }

    private T value;
    private readonly IGraphicsDevice device;

    public RenderParameter(string name, IGraphicsDevice device, IBuffer resource)
        : base(name, resource)
    {
        this.device = device;
    }
}
