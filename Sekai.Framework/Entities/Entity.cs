// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Sekai.Framework.Services;

namespace Sekai.Framework.Entities;

[Cached]
public class Entity : LoadableObject
{
    /// <summary>
    /// The parent entity.
    /// </summary>
    public Entity? Parent { get; private set; }

    /// <summary>
    /// The identifier for this entity.
    /// </summary>
    public readonly Guid ID = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the priority of this entity.
    /// </summary>
    /// <remarks>
    /// This affects the order in which when the entity is processed.
    /// </remarks>
    public int Priority { get; set; }

    public override IServiceContainer Services => services;

    /// <summary>
    /// Gets how far this entity is from the root.
    /// </summary>
    internal int Depth { get; private set; }

    /// <summary>
    /// Gets the calculated ordering for this entity for processing.
    /// </summary>
    internal int Order => Depth + Priority;

    private readonly ProtectedServiceContainer services = new();
    private readonly List<string> tags = new();
    private readonly List<Entity> children = new();
    private readonly List<Component> components = new();

    /// <summary>
    /// Gets or sets this entity's children.
    /// </summary>
    public IEnumerable<Entity> Children
    {
        get => children;
        set
        {
            RemoveRange(children);
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets this entity's components.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => components;
        set
        {
            RemoveRange(components);
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets the name of this entity.
    /// </summary>
    public string Name { get; set; } = "Entity";

    /// <summary>
    /// Gets or sets the only tag for this entity.
    /// </summary>
    public string Tag
    {
        get => tags.Single();
        set
        {
            tags.Clear();
            tags.Add(value);
        }
    }

    /// <summary>
    /// Gets or sets the tags for this entity.
    /// </summary>
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

    protected override void OnLoad()
    {
        services.IsProtected = true;

        foreach (var child in children)
            scene.Add(child, false);

        foreach (var component in components)
            scene.RegisterComponent(component);
    }

    /// <summary>
    /// Adds an entity as its child and loads it.
    /// </summary>
    public void Add(Entity entity)
    {
        if (entity.Parent != null)
            throw new InvalidOperationException(@"Cannot add an entity that already has a parent.");

        lock (children)
        {
            if (children.Contains(entity))
                return;

            entity.Depth = Depth + 1;
            entity.Parent = this;
            AddInternal(entity);
            children.Add(entity);
            scene?.Add(entity, false);
        }
    }

    /// <summary>
    /// Removes an entity as its child and unloads it.
    /// </summary>
    public void Remove(Entity entity)
    {
        if (entity.Parent == null)
            throw new InvalidOperationException(@"This entity is currently not attached to another.");

        lock (children)
        {
            if (!children.Contains(entity))
                return;

            entity.Depth = 0;
            entity.Parent = null;
            RemoveInternal(entity);
            children.Remove(entity);
            scene?.Remove(entity, false);
        }
    }

    /// <summary>
    /// Adds a component to this entity and loads it.
    /// </summary>
    public void Add(Component component)
    {
        if (component.Entity != null)
            throw new InvalidOperationException(@"This component is currently attached to an entity.");

        lock (components)
        {
            if (components.Contains(component))
                return;

            component.Entity = this;
            components.Add(component);
            components.Sort(ComponentComparer.Instance);
            AddInternal(component);
            scene?.RegisterComponent(component);
        }
    }

    /// <summary>
    /// Removes a component from this entity and unloads it.
    /// </summary>
    public void Remove(Component component)
    {
        if (component.Entity == null)
            throw new InvalidOperationException(@"This component is currently not attached to an entity.");

        lock (components)
        {
            if (!components.Contains(component))
                return;

            component.Entity = null!;
            components.Remove(component);
            components.Sort(ComponentComparer.Instance);
            RemoveInternal(component);
        }
    }

    /// <summary>
    /// Adds a range of entities as children and loads those.
    /// </summary>
    public void AddRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Add(entity);
    }

    /// <summary>
    /// Adds a range of components to this entity and loads those.
    /// </summary>
    public void AddRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
            Add(component);
    }

    /// <summary>
    /// Removes a range of entities and unloads those.
    /// </summary>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Remove(entity);
    }

    /// <summary>
    /// Removes a range of components and unloads those.
    /// </summary>
    public void RemoveRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
            Remove(component);
    }

    /// <summary>
    /// Gets all components as a given type.
    /// </summary>
    public IEnumerable<T> GetComponents<T>()
        where T : Component
    {
        return components.OfType<T>();
    }

    /// <summary>
    /// Gets the only component of a given type.
    /// </summary>
    public T GetComponent<T>()
        where T : Component
    {
        return GetComponents<T>().Single();
    }

    /// <inheritdoc cref="GetComponents{T}"/>
    public IEnumerable<Component> GetComponents(Type type)
    {
        return components.Where(c => c.GetType().IsAssignableTo(type));
    }

    /// <inheritdoc cref="GetComponent{T}"/>
    public Component GetComponent(Type type)
    {
        return GetComponents(type).Single();
    }

    private class ComponentComparer : IComparer<Component>
    {
        public static readonly ComponentComparer Instance = new();

        public int Compare(Component? x, Component? y)
        {
            int a = x?.Priority ?? 0;
            int b = y?.Priority ?? 0;
            return a.CompareTo(b);
        }
    }

    private class ProtectedServiceContainer : FrameworkObject, IServiceContainer
    {
        public bool IsProtected { get; set; }
        private readonly ServiceContainer underlyingContainer = new();

        public IReadOnlyServiceContainer? Parent
        {
            get => underlyingContainer.Parent;
            set => underlyingContainer.Parent = value;
        }

        public void Cache(Type type, Func<object> creationFunc)
        {
            if (IsProtected)
                return;

            underlyingContainer.Cache(type, creationFunc);
        }

        public void Cache(Type type, object instance)
        {
            if (IsProtected)
                return;

            underlyingContainer.Cache(type, instance);
        }

        public void Cache<T>(Func<T> creationFunc)
        {
            if (IsProtected)
                return;

            underlyingContainer.Cache(creationFunc);
        }

        public void Cache<T>(T instance)
        {
            if (IsProtected)
                return;

            underlyingContainer.Cache(instance);
        }

        public T Resolve<T>([DoesNotReturnIf(true)] bool required = false)
        {
            return underlyingContainer.Resolve<T>(required);
        }

        public object Resolve(Type type, [DoesNotReturnIf(true)] bool required = false)
        {
            return underlyingContainer.Resolve(type, required);
        }
    }
}
