// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;

namespace Sekai.Graphics;

public abstract partial class ServiceableGraphicsObject<T> : ServiceableObject
{
    [Resolved]
    private GraphicsContext context { get; set; } = null!;

    internal T Native => nativeObject.Native;

    private readonly GraphicsObject<T> nativeObject;

    protected ServiceableGraphicsObject(Func<GraphicsContext, GraphicsObject<T>> creator)
    {
        nativeObject = creator(context);
    }

    protected sealed override void Dispose(bool disposing)
    {
        if (disposing)
            nativeObject.Dispose();

        base.Dispose(disposing);
    }
}
