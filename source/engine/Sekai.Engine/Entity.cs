// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework;

namespace Sekai.Engine;

public class Entity : FrameworkObject, IReference, IEquatable<Entity>
{
    /// <summary>
    /// The identifier for this entity.
    /// </summary>
    public Guid Id { get; internal set; }

    /// <summary>
    /// The identifier of the parent entity for this entity.
    /// </summary>
    public Entity? Parent { get; internal set; }

    /// <summary>
    /// The scene owning this entity.
    /// </summary>
    public Scene Scene { get; internal set; } = null!;

    /// <summary>
    /// Creates a new child entity.
    /// </summary>
    public Entity CreateEntity()
    {
        return Scene.CreateEntity(this);
    }

    /// <summary>
    /// Gets this entity's children.
    /// </summary>
    public IEnumerable<Entity> GetChildren()
    {
        return Scene.GetChildren(this);
    }

    /// <summary>
    /// Removes all of this entity's children.
    /// </summary>
    public void Clear()
    {
        Scene.ClearChildren(this);
    }

    /// <summary>
    /// Adds a component to this entity.
    /// </summary>
    public Entity AddComponent<T>()
        where T : Component
    {
        Scene.AddComponent(this, Activator.CreateInstance<T>());
        return this;
    }

    /// <summary>
    /// Adds a component to this entity.
    /// </summary>
    public Entity AddComponent<T>(T component)
        where T : Component
    {
        Scene.AddComponent(this, component);
        return this;
    }

    /// <summary>
    /// Removes a component from this entity.
    /// </summary>
    public Entity RemoveComponent<T>()
        where T : Component
    {
        Scene.RemoveComponent<T>(this);
        return this;
    }

    /// <summary>
    /// Gets all the components owned by this entity.
    /// </summary>
    public IEnumerable<Component> GetComponents()
    {
        return Scene.GetComponents(this);
    }

    /// <summary>
    /// Gets a component of a certain type owned by this entity.
    /// </summary>
    public T? GetCommponent<T>()
        where T : Component
    {
        return Scene.GetComponent<T>(this);
    }

    /// <summary>
    /// Removes all of this entity's components.
    /// </summary>
    public void ClearComponents()
    {
        Scene.RemoveComponents(this);
    }

    protected override void Destroy()
    {
        Scene.RemoveEntity(this);
    }

    public bool Equals(Entity? other)
    {
        return other is not null
            && other.Id == Id
            && other.IsDisposed == IsDisposed
            && (other.Parent?.Equals(this) ?? false);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsDisposed, Id, Parent, Scene);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity);
    }
}
