// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

public abstract class GraphicsObject : DisposableObject
{
    private readonly IDisposable token;

    public GraphicsObject(IDisposable token)
    {
        this.token = token;
    }

    protected sealed override void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        token.Dispose();
    }
}

public sealed class GraphicsObject<T> : GraphicsObject
{
    public T Native { get; }

    public GraphicsObject(IDisposable token, T native)
        : base(token)
    {
        Native = native;
    }
}
