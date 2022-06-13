// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Extensions;
using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

[Cached]
public class Scene : LoadableObject
{
    public event Action<Entity>? OnEntityAdded;
    public event Action<Entity>? OnEntityRemoved;
    public IReadOnlyList<Entity> Entities => rootEntities;
    internal IReadOnlyList<Entity> AllAliveEntities => allAliveEntities;
    private readonly List<Entity> rootEntities = new();
    private readonly List<Entity> allAliveEntities = new();

    public void Add(Entity entity)
    {
        Add(entity, true);
    }

    public void Remove(Entity entity)
    {
        Add(entity, true);
    }

    public IEnumerable<Entity> GetEntitiesByTag(string tag)
    {
        return allAliveEntities.Where(e => e.Tags.Contains(tag));
    }

    internal void Add(Entity entity, bool root)
    {
        if (root)
        {
            rootEntities.Add(entity);
            this.Add((LoadableObject)entity);
        }

        allAliveEntities.Add(entity);
        OnEntityAdded?.Invoke(entity);
    }

    internal void Remove(Entity entity, bool root)
    {
        if (root)
        {
            rootEntities.Remove(entity);
            this.Remove((LoadableObject)entity);
        }

        allAliveEntities.Remove(entity);
        OnEntityRemoved?.Invoke(entity);
    }
}
