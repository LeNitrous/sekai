// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
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
    /// The underlying graphics resource.
    /// </summary>
    internal abstract T Resource { get; }

    protected override void Destroy()
    {
        if (Resource is IDisposable disposable)
            disposable.Dispose();
    }
}

/// <summary>
/// An object capable of holding one or more resources.
/// </summary>
public abstract class GraphicsObjectCollection<T> : GraphicsObject
    where T : DeviceResource
{
    /// <summary>
    /// The underlying graphics resources.
    /// </summary>
    internal abstract IReadOnlyList<T> Resources { get; }

    protected override void Destroy()
    {
        foreach (var res in Resources)
        {
            if (res is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
