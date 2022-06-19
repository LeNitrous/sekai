// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Veldrid;

namespace Sekai.Framework.Graphics;

/// <summary>
/// An object capable of interacting with the graphics context.
/// </summary>
/// <remarks>
/// Objects derived from <see cref="GraphicsObject"/> can only exist within the <see cref="Platform.Host"/>'s lifetime.
/// </remarks>
public abstract class GraphicsObject : FrameworkObject
{
    /// <summary>
    /// The current graphics context.
    /// </summary>
    internal readonly GraphicsContext Context;

    public GraphicsObject()
    {
        if (Game.Current == null)
            throw new InvalidOperationException(@"An instance of a game must be created to be able to create graphics objects.");

        Context = (GraphicsContext)Game.Current.Services.Resolve<IGraphicsContext>();
    }
}

/// <inheritdoc cref="GraphicsObject"/>
public abstract class GraphicsObject<T> : GraphicsObject
    where T : DeviceResource
{
    /// <summary>
    /// The underlying graphics resource that is lazily initialized.
    /// </summary>
    internal T Resource => resource.Value;
    private readonly Lazy<T> resource;

    public GraphicsObject()
    {
        resource = new Lazy<T>(CreateResource);
    }

    /// <summary>
    /// Creates the resource for this graphics object.
    /// </summary>
    protected abstract T CreateResource();
    protected override void Destroy() => (Resource as IDisposable)?.Dispose();
}
