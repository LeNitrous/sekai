// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Sekai.Framework.Scenes;

namespace Sekai.Framework;

/// <summary>
/// A scene object that is capable of owning its own children and is composable through <see cref="Component"/>s.
/// </summary>
public sealed class Entity
{
    /// <summary>
    /// The parent entity.
    /// </summary>
    public Entity? Parent
    {
        get
        {
            if (!IsAttached)
                throw new InvalidOperationException("The entity is not attached to a scene.");

            return parent;
        }
    }

    /// <summary>
    /// The scene where this entity is attached to.
    /// </summary>
    public Scene Scene
    {
        get
        {
            if (!IsAttached)
                throw new InvalidOperationException("The entity is not attached to a scene.");

            return scene;
        }
    }

    /// <summary>
    /// Gets or sets this entity's children.
    /// </summary>
    public IEnumerable<Entity> Children
    {
        get => children;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets this entity's components.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => components?.Where(c => c != null) ?? Array.Empty<Component>();
        set
        {
            ClearComponents();
            AddComponentRange(value);
        }
    }

    /// <summary>
    /// Called when a child entity has been added.
    /// </summary>
    public event Action<Entity, Entity> OnChildAdded = null!;

    /// <summary>
    /// Called when a child entity has been removed.
    /// </summary>
    public event Action<Entity, Entity> OnChildRemoved = null!;

    /// <summary>
    /// Called when a component has been added.
    /// </summary>
    public event Action<Entity, Component> OnComponentAdded = null!;

    /// <summary>
    /// Called when a component has been removed.
    /// </summary>
    public event Action<Entity, Component> OnComponentRemoved = null!;

    internal bool IsAttached { get; private set; }

    internal Transform? Transform { get; private set; }

    private Scene scene = null!;
    private Entity? parent;
    private int componentCount;
    private Component[]? components;
    private readonly List<Entity> children = new();
    private readonly Dictionary<Type, int> indices = new();

    /// <summary>
    /// Adds a child entity.
    /// </summary>
    public void Add(Entity entity)
    {
        if (entity.Parent != null)
            throw new InvalidOperationException(@"Entity already has a parent attached.");

        children.Add(entity);

        if (IsAttached)
        {
            entity.Attach(Scene, this);
            OnChildAdded?.Invoke(this, entity);
        }
    }

    /// <summary>
    /// Adds a range of entities as children.
    /// </summary>
    public void AddRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Add(entity);
    }

    /// <summary>
    /// Removes a child entity.
    /// </summary>
    public void Remove(Entity entity)
    {
        if(!children.Remove(entity))
            return;

        if (IsAttached)
        {
            OnChildRemoved?.Invoke(this, entity);
            entity.Detach();
        }
    }

    /// <summary>
    /// Removes a range of entities from its hierarchy.
    /// </summary>
    public void RemoveRange(IEnumerable<Entity> entities)
    {
        foreach (var entity in entities)
            Remove(entity);
    }

    /// <summary>
    /// Removes all child entities.
    /// </summary>
    public void Clear()
    {
        RemoveRange(children.ToArray());
    }

    /// <summary>
    /// Adds a component to this entity.
    /// </summary>
    public void AddComponent(Component component)
    {
        if (component.IsAttached)
            throw new InvalidOperationException(@"Component is already attached to an entity.");

        var type = component.GetType();

        if (indices.ContainsKey(type))
            throw new InvalidOperationException($"A component of type {type} already exists in this entity.");

        components ??= new Component[16];

        if (componentCount + 1 > components.Length)
            Array.Resize(ref components, components.Length >> 1);

        indices.Add(type, componentCount);
        components[componentCount] = component;

        componentCount++;

        if (component is Transform transform)
        {
            Transform = transform;
        }

        if (IsAttached)
        {
            component.Attach(Scene, this);
            OnComponentAdded?.Invoke(this, component);
        }
    }

    /// <summary>
    /// Adds a component to this entity of a certain type.
    /// </summary>
    /// <remarks>
    /// The component type must have a default parameterless constructor.
    /// </remarks>
    public void AddComponent<T>()
        where T : Component, new()
    {
        AddComponent(Activator.CreateInstance<T>());
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
    /// Removes a component from this entity.
    /// </summary>
    public void RemoveComponent(Component component)
    {
        if (components == null)
            return;

        if (!indices.TryGetValue(component.GetType(), out int index))
            return;

        var temp = components[^1];

        components[index] = temp;
        components[^1] = null!;

        if (temp != component)
            indices[temp.GetType()] = index;

        componentCount--;

        if (component is Transform)
        {
            Transform = null;
        }

        if (IsAttached)
        {
            OnComponentRemoved?.Invoke(this, component);
            component.Detach();
        }
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
        if (components == null)
            return;

        indices.Clear();
        Array.Clear(components);
        componentCount = 0;
    }

    /// <summary>
    /// Gets the component from this entity.
    /// </summary>
    public Component? GetComponent(Type type)
    {
        if (components is null || !indices.TryGetValue(type, out int index))
            return null;

        return components[index];
    }

    /// <summary>
    /// Gets the component from this entity.
    /// </summary>
    public T? GetComponent<T>()
        where T : Component
    {
        // The "as" operator performs type checking which introduces
        // unnecessary overhead as we already know the type we're retrieving.
        return Unsafe.As<T>(GetComponent(typeof(T)));
    }

    /// <summary>
    /// Gets whether the entity has a component of a certain type.
    /// </summary>
    public bool HasComponent(Type type)
    {
        return indices.ContainsKey(type);
    }

    /// <summary>
    /// Gets whether the entity has a component of a certain type.
    /// </summary>
    public bool HasComponent<T>()
        where T : Component
    {
        return HasComponent(typeof(T));
    }

    internal void Attach(Scene scene, Entity? parent)
    {
        if (IsAttached)
            return;

        this.scene = scene;
        this.parent = parent;
        IsAttached = true;

        Scene.Attach(this);

        if (components != null)
        {
            foreach (var component in components)
            {
                if (component is null)
                    continue;

                component.Attach(Scene, this);
                OnComponentAdded?.Invoke(this, component);
            }
        }

        foreach (var child in children)
        {
            child.Attach(scene, parent);
            OnChildAdded?.Invoke(this, child);
        }
    }

    internal void Detach()
    {
        if (!IsAttached)
            return;

        Scene.Detach(this);

        IsAttached = false;
        scene = null!;
        parent = null;

        foreach (var child in children)
        {
            OnChildRemoved?.Invoke(this, child);
            child.Detach();
        }

        if (components != null)
        {
            foreach (var component in components)
            {
                if (component is null)
                    continue;

                OnComponentRemoved?.Invoke(this, component);
                component.Detach();
            }
        }
    }
}
