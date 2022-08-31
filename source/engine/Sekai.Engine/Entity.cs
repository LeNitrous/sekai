// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Annotations;

namespace Sekai.Engine;

[Cached]
public class Entity : EntityCollection
{
    /// <summary>
    /// The parent entity.
    /// </summary>
    public new Entity? Parent => base.Parent as Entity;

    /// <summary>
    /// The current scene.
    /// </summary>
    [Resolved]
    public Scene Scene { get; internal set; } = null!;

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
    public IEnumerable<string> Tags
    {
        get => tags ?? (Array.Empty<string>() as IEnumerable<string>);
        set
        {
            tags ??= new();
            tags.Clear();
            tags.AddRange(value);
        }
    }


    /// <summary>
    /// Gets or sets the components of this entity.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => Loadables.OfType<Component>();
        set
        {
            ClearComponents();
            AddComponentRange(value);
        }
    }

    /// <summary>
    /// Gets or sets this entity's name.
    /// </summary>
    public string Name { get; set; } = "unnamed";

    private List<string> tags = null!;

    protected sealed override void Load()
    {
        Scene.OnEntityUpdate(this);
    }

    protected sealed override void Unload()
    {
        Scene.OnEntityUpdate(this);
    }

    protected sealed override void Activate()
    {
        Scene.OnEntityUpdate(this);
    }

    protected sealed override void Deactivate()
    {
        Scene.OnEntityUpdate(this);
    }

    /// <summary>
    /// Adds a component to this entity.
    /// </summary>
    public void AddComponent(Component component)
    {
        AddInternal(component);
    }

    /// <summary>
    /// Removes a component from this entity.
    /// </summary>
    public void RemoveComponent(Component component)
    {
        RemoveInternal(component);
    }

    /// <summary>
    /// Adds a range of components to this entity.
    /// </summary>
    public void AddComponentRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
            AddComponent(component);
    }

    /// <summary>
    /// Removes a range of components from this entity.
    /// </summary>
    public void RemoveComponentRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
            RemoveComponent(component);
    }

    /// <summary>
    /// Removes all components from this entity.
    /// </summary>
    public void ClearComponents()
    {
        RemoveComponentRange(Components);
    }

    /// <summary>
    /// Gets all components of a given type.
    /// </summary>
    public IEnumerable<Component> GetComponents(Type type)
    {
        if (!type.IsAssignableTo(typeof(Component)) || type.IsAbstract)
            throw new InvalidCastException(@$"Type must be a non-abstract {nameof(Component)}.");

        return Components.Where(c => c.GetType().Equals(type));
    }

    /// <summary>
    /// Gets all components of a given type.
    /// </summary>
    public IEnumerable<T> GetComponents<T>()
        where T : Component
    {
        return Components.OfType<T>();
    }

    /// <summary>
    /// Gets a single component of a given type.
    /// </summary>
    public T? GetComponent<T>()
        where T : Component
    {
        return GetComponents<T>().SingleOrDefault();
    }

    /// <summary>
    /// Returns whether this entity has a component of a given type.
    /// </summary>
    public bool HasComponent(Type type)
    {
        return GetComponents(type).Any();
    }

    /// <summary>
    /// Returns whether this entity has a component of a given type.
    /// </summary>
    public bool HasComponent<T>()
        where T : Component
    {
        return GetComponents<T>().Any();
    }
}
