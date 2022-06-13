// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Extensions;
using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

[Cached]
public class Entity : LoadableObject
{
    public Entity? Parent { get; set; }
    public IReadOnlyList<Entity> Children => children;
    public IReadOnlyList<Component> Components => components;
    public readonly Guid ID = Guid.NewGuid();
    private readonly List<string> tags = new();
    private readonly List<Entity> children = new();
    private readonly List<Component> components = new();

    public string Tag
    {
        get => tags.Single();
        set => tags[0] = value;
    }

    public IEnumerable<string> Tags
    {
        get => tags;
        set
        {
            tags.Clear();
            tags.AddRange(value);
        }
    }

    [Resolved]
    private Scene scene { get; set; } = null!;

    public void Add(Entity entity)
    {
        children.Add(entity);
        this.Add((LoadableObject)entity);
        scene.Remove(entity, false);
    }

    public void Remove(Entity entity)
    {
        children.Remove(entity);
        this.Remove((LoadableObject)entity);
        scene.Remove(entity, false);
    }

    public void Add(Component component)
    {
        components.Add(component);
        this.Add((LoadableObject)component);
    }

    public void Remove(Component component)
    {
        components.Remove(component);
        this.Remove((LoadableObject)component);
    }

    public IEnumerable<T> GetComponents<T>()
        where T : Component
    {
        return components.OfType<T>();
    }

    public T GetComponent<T>()
        where T : Component
    {
        return GetComponents<T>().Single();
    }

    public IEnumerable<Component> GetComponents(Type type)
    {
        return components.Where(c => c.GetType().IsAssignableTo(type));
    }

    public Component GetComponent(Type type)
    {
        return GetComponents(type).Single();
    }
}
