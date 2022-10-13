// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework;

namespace Sekai.Engine;

public abstract class Component : FrameworkObject, IReference, IEquatable<Component>
{
    /// <summary>
    /// The identifier for this component.
    /// </summary>
    public Guid Id { get; internal set; }

    /// <summary>
    /// The entity owning this component.
    /// </summary>
    public Entity Entity { get; internal set; } = null!;

    /// <summary>
    /// The scene owning this component.
    /// </summary>
    public Scene Scene { get; internal set; } = null!;

    public bool Equals(Component? other)
    {
        return other is not null
            && other.Id == Id
            && other.Entity.Equals(Entity)
            && other.IsDisposed == IsDisposed
            && EqualityComparer<Scene>.Default.Equals(other.Scene, Scene);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsDisposed, Id, Entity, Scene);
    }

    protected sealed override void Destroy()
    {
        Scene.RemoveComponent(this);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Component);
    }
}
