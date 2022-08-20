// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

[Cached]
public class Entity : ActivatableObject
{
    /// <summary>
    /// Gets the parent entity.
    /// </summary>
    public new Entity Parent => (base.Parent as Entity)!;

    /// <summary>
    /// Gets or sets the children for this entity.
    /// </summary>
    public IEnumerable<Entity> Entities
    {
        get => Loadables.OfType<Entity>();
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// The transform of the entity
    /// </summary>
    public Transform? Transform { get; set; }

    /// <summary>
    /// Gets or sets the components for this entity.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => Loadables.OfType<Component>();
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets the only tag for this entity.
    /// </summary>
    public string Tag
    {
        get => tags?.FirstOrDefault() ?? string.Empty;
        set
        {
            tags ??= new();
            tags.Clear();
            tags.Add(value);
        }
    }

    /// <summary>
    /// Gets or sets the tags for this entity.
    /// </summary>
    public IReadOnlyList<string> Tags
    {
        get => tags ?? (Array.Empty<string>() as IReadOnlyList<string>);
        set
        {
            tags ??= new();
            tags.Clear();
            tags.AddRange(value);
        }
    }

    [Resolved]
    private Scene scene { get; set; } = null!;

    private List<string> tags = null!;

    protected override void Load()
    {
        foreach (var entity in Entities)
            scene.AddAliveEntity(entity);
    }

    protected override void Unload()
    {
        foreach (var entity in Entities)
            scene.RemoveAliveEntity(entity);
    }

    /// <summary>
    /// Adds a child to this entity.
    /// </summary>
    public void Add(Entity entity)
    {
        AddInternal(entity);
        scene?.AddAliveEntity(entity);
    }

    /// <summary>
    /// Removes a child from this entity.
    /// </summary>
    public void Remove(Entity entity)
    {
        RemoveInternal(entity);
        scene?.RemoveAliveEntity(entity);
    }

    /// <summary>
    /// Adds a range of children to this entity.
    /// </summary>
    public void AddRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Add(entity);
    }

    /// <summary>
    /// Removes a range of children from this entity.
    /// </summary>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Remove(entity);
    }

    /// <summary>
    /// Removes all children in this entity.
    /// </summary>
    public void Clear()
    {
        RemoveRange(Entities);
    }

    /// <summary>
    /// Adds a component to this entity.
    /// </summary>
    public void Add(Component component)
    {
        AddInternal(component);
    }

    /// <summary>
    /// Removes a component from this entity.
    /// </summary>
    public void Remove(Component component)
    {
        RemoveInternal(component);
    }

    /// <summary>
    /// Adds a range of components to this entity.
    /// </summary>
    public void AddRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
            Add(component);
    }

    /// <summary>
    /// Removes a range of components from this entity.
    /// </summary>
    public void RemoveRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
            Remove(component);
    }

    /// <summary>
    /// Removes all components from this entity.
    /// </summary>
    public void ClearComponents()
    {
        RemoveRange(Components);
    }

    /// <summary>
    /// Gets components of a given type.
    /// </summary>
    public IEnumerable<Component> GetComponents(Type type)
    {
        return Components.Where(c => c.GetType().IsAssignableTo(type));
    }

    /// <summary>
    /// Gets a single component of a given type.
    /// </summary>
    public Component GetComponent(Type type)
    {
        return GetComponents(type).Single();
    }

    /// <summary>
    /// Gets components of a given type.
    /// </summary>
    public IEnumerable<T> GetComponents<T>()
        where T : Component
    {
        return Loadables.OfType<T>();
    }

    /// <summary>
    /// Gets a single component of a given type.
    /// </summary>
    public T GetComponent<T>()
        where T : Component
    {
        return GetComponents<T>().Single();
    }

    /// <summary>
    /// Gets whether this entity has a component of a given type.
    /// </summary>
    public bool HasComponent(Type type)
    {
        return GetComponents(type).Any();
    }

    /// <summary>
    /// Gets whether this entity as a component of a given type.
    /// </summary>
    public bool HasComponent<T>()
        where T : Component
    {
        return GetComponents<T>().Any();
    }
}
