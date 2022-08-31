// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine;

public abstract class EntityCollection : ActivatableObject
{
    /// <summary>
    /// Gets or sets the children of this entity.
    /// </summary>
    public IEnumerable<Entity> Children
    {
        get => Loadables.OfType<Entity>();
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Adds an entity as its child.
    /// </summary>
    public virtual void Add(Entity entity)
    {
        AddInternal(entity);
    }

    /// <summary>
    /// Removes a child entity from itself.
    /// </summary>
    public virtual void Remove(Entity entity)
    {
        RemoveInternal(entity);
    }

    /// <summary>
    /// Adds a range of entities as its children.
    /// </summary>
    public void AddRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Add(entity);
    }

    /// <summary>
    /// Removes a range of children entities from itself.
    /// </summary>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Remove(entity);
    }

    /// <summary>
    /// Removes all children entities from itself.
    /// </summary>
    public void Clear()
    {
        RemoveRange(Children);
    }
}
