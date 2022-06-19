// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sekai.Framework.Entities.Processors;
using Sekai.Framework.Graphics;
using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

[Cached]
public class Scene : LoadableObject, IUpdateable, IRenderable
{
    /// <summary>
    /// The display name for this scene.
    /// </summary>
    public virtual string Name { get; set; } = "Scene";

    /// <summary>
    /// The scene manager that owns this scene.
    /// </summary>
    public SceneManager? Manager { get; internal set; }

    private readonly Dictionary<Type, EntityProcessor?> processors = new();
    private readonly List<Entity> rootEntities = new();
    private readonly List<Entity> allAliveEntities = new();

    /// <summary>
    /// Sets or gets the current entities for this scene.
    /// </summary>
    public IEnumerable<Entity> Entities
    {
        get => rootEntities;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Adds an entity to this scene.
    /// </summary>
    public void Add(Entity entity)
    {
        Add(entity, true);
    }

    /// <summary>
    /// Removes an entity from this scene.
    /// </summary>
    public void Remove(Entity entity)
    {
        Add(entity, true);
    }

    /// <summary>
    /// Adds a range of entities to this scene.
    /// </summary>
    public void AddRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Add(entity);
    }

    /// <summary>
    /// Removes a range of entities from this scene.
    /// </summary>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Remove(entity);
    }

    /// <summary>
    /// Removes all entities from this scene.
    /// </summary>
    public void Clear()
    {
        RemoveRange(rootEntities);
    }

    /// <summary>
    /// Gets all entities by a given tag.
    /// </summary>
    public IEnumerable<Entity> GetEntitiesByTag(string tag)
    {
        return allAliveEntities.Where(e => e.Tags.Contains(tag));
    }

    /// <summary>
    /// Gets an entity by name.
    /// </summary>
    public Entity GetEntityByName(string name)
    {
        return allAliveEntities.Single(e => e.Name == name);
    }

    internal void Add(Entity entity, bool root)
    {
        if (root)
        {
            lock (rootEntities)
            {
                if (rootEntities.Contains(entity))
                    return;

                AddInternal(entity);
                rootEntities.Add(entity);
            }

        }

        lock (allAliveEntities)
        {
            if (!allAliveEntities.Contains(entity))
            {
                allAliveEntities.Add(entity);
                allAliveEntities.Sort(EntityComparer.Instance);
            }
        }
    }

    internal void Remove(Entity entity, bool root)
    {
        if (root)
        {
            lock (rootEntities)
            {
                if (!rootEntities.Contains(entity))
                    return;

                RemoveInternal(entity);
                rootEntities.Remove(entity);
            }
        }

        lock (allAliveEntities)
        {
            if (!allAliveEntities.Contains(entity))
            {
                allAliveEntities.Remove(entity);
                allAliveEntities.Sort(EntityComparer.Instance);
            }
        }
    }

    internal void RegisterComponent(Component component)
    {
        var type = component.GetType();

        lock (processors)
        {
            if (processors.ContainsKey(type))
                return;

            EntityProcessor? processor = null;

            var attrib = type.GetCustomAttribute<EntityProcessorAttribute>();

            if (attrib != null)
                processor = Activator.CreateInstance(attrib.ProcessorType) as EntityProcessor;

            processors.Add(type, processor);
        }
    }

    void IRenderable.Render(CommandList commands)
    {
        var processableEntities = allAliveEntities.OrderBy(e => e.Order).ToArray();

        foreach (var processor in processors.Values)
        {
            if (processor == null)
                continue;

            foreach (var entity in processableEntities)
            {
                processor.Render(entity, commands);
            }
        }
    }

    void IUpdateable.Update(double elapsed)
    {
        var processableEntities = allAliveEntities.OrderBy(e => e.Order).ToArray();

        foreach (var processor in processors.Values)
        {
            if (processor == null)
                continue;

            foreach (var entity in processableEntities)
            {
                processor.Update(entity, elapsed);
            }
        }
    }

    private class EntityComparer : IComparer<Entity>
    {
        public static readonly EntityComparer Instance = new();

        public int Compare(Entity? x, Entity? y)
        {
            int a = x?.Order ?? 0;
            int b = y?.Order ?? 0;
            return a.CompareTo(b);
        }
    }
}
