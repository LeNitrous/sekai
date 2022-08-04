// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

[Cached]
public class Scene : ActivatableObject
{
    /// <summary>
    /// Gets or sets the name for this scene.
    /// </summary>
    public string Name { get; set; } = "Scene";

    /// <summary>
    /// Gets the manager owning this scene.
    /// </summary>
    public SceneManager Manager { get; internal set; } = null!;

    /// <summary>
    /// Gets or sets the entities for this scene.
    /// </summary>
    public IEnumerable<Entity> Entities
    {
        get => root.Entities;
        set => root.Entities = value;
    }

    /// <summary>
    /// Gets all alive entities for this scene.
    /// </summary>
    internal IReadOnlyCollection<Entity> AllAliveEntities => allAliveEntities;

    private readonly Entity root;
    private readonly HashSet<Entity> allAliveEntities = new();

    public Scene()
    {
        AddInternal(root = new());
    }

    internal void AddAliveEntity(Entity entity)
    {
        allAliveEntities.Add(entity);
    }

    internal void RemoveAliveEntity(Entity entity)
    {
        allAliveEntities.Remove(entity);
    }

    /// <summary>
    /// Adds an entity to this scene.
    /// </summary>
    public void Add(Entity entity)
    {
        root.Add(entity);
    }

    /// <summary>
    /// Removes an entity from this scene.
    /// </summary>
    public void Remove(Entity entity)
    {
        root.Remove(entity);
    }

    /// <summary>
    /// Adds a range of entities to this scene.
    /// </summary>
    public void AddRange(IEnumerable<Entity> entities)
    {
        root.AddRange(entities);
    }

    /// <summary>
    /// Removes a range of entities from this scene.
    /// </summary>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        root.RemoveRange(entities);
    }
}
