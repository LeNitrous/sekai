// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Veldrid;

namespace Sekai.Framework.Graphics;
public abstract class GraphicsObject<T> : FrameworkObject
    where T : DeviceResource
{
    internal readonly GraphicsContext Context = (GraphicsContext)Game.Current.Services.Resolve<IGraphicsContext>();
    protected T Resource => resource.Value;
    private readonly Lazy<T> resource;

    public GraphicsObject()
    {
        resource = new Lazy<T>(CreateResource);
    }

    protected abstract T CreateResource();

    protected override void Destroy() => (Resource as IDisposable)?.Dispose();
}
