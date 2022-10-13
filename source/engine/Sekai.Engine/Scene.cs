// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Engine.Processors;
using Sekai.Engine.Rendering;
using Sekai.Framework;

namespace Sekai.Engine;

public class Scene : FrameworkObject, IUpdateable, IRenderable
{
    /// <summary>
    /// Gets or sets the name for this scene.
    /// </summary>
    public string Name { get; set; } = "Scene";

    /// <summary>
    /// Called when an entity has been added to the scene.
    /// </summary>
    public event Action<Scene, Entity> OnEntityAdded = null!;

    /// <summary>
    /// Called when an entity has been removed from the scene.
    /// </summary>
    public event Action<Scene, Entity> OnEntityRemoved = null!;

    /// <summary>
    /// Called when a component has been added to an entity.
    /// </summary>
    public event Action<Scene, Entity, Component> OnComponentAdded = null!;

    /// <summary>
    /// Called when a component has been removed from an entity.
    /// </summary>
    public event Action<Scene, Entity, Component> OnComponentRemoved = null!;

    public readonly RenderContext RenderContext;

    private readonly HashSet<Entity> entities = new();
    private readonly HashSet<Component> components = new();
    private readonly SystemCollection<SceneSystem> systems = new();

    public Scene()
    {
        RenderContext = Register<RenderContext>();
        Register<MeshProcessor>();
        Register<CameraProcessor>();
        Register<BehaviorProcessor>();
        Register<TransformProcessor>();
    }

    /// <summary>
    /// Registers a scene system for this scene.
    /// </summary>
    public T Register<T>()
        where T : SceneSystem
    {
        var system = systems.Register<T>();
        system.Scene = this;
        system.Initialize();
        return system;
    }

    public void Unregister<T>()
        where T : SceneSystem
    {
        systems.Unregister<T>();
    }

    /// <summary>
    /// Creates a new entity for this scene.
    /// </summary>
    public Entity CreateEntity()
    {
        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Scene = this,
        };

        entities.Add(entity);
        return entity;
    }

    /// <summary>
    /// Removes an entity from this scene.
    /// </summary>
    public bool RemoveEntity(Entity target)
    {
        bool result = entities.Remove(target);

        if (!result)
            return false;

        OnEntityRemoved?.Invoke(this, target);

        ClearChildren(target);
        RemoveComponents(target);

        foreach (var component in GetComponents(target))
            RemoveComponent(component);

        return true;
    }

    /// <summary>
    /// Removes all entities from this scene.
    /// </summary>
    public void Clear()
    {
        foreach (var entity in entities)
            RemoveEntity(entity);
    }

    /// <summary>
    /// Removes all children from the given entity.
    /// </summary>
    internal void ClearChildren(Entity target)
    {
        foreach (var child in GetChildren(target))
            RemoveEntity(child);
    }

    /// <summary>
    /// Creates a new entity with a specified parent.
    /// </summary>
    internal Entity CreateEntity(Entity parent)
    {
        var entity = new Entity
        {
            Id = Guid.NewGuid(),
            Scene = this,
            Parent = parent,
        };

        entities.Add(entity);
        OnEntityAdded?.Invoke(this, entity);

        return entity;
    }

    /// <summary>
    /// Gets the children entities of a given entity.
    /// </summary>
    internal IEnumerable<Entity> GetChildren(Entity target)
    {
        return entities.Where(e => e.Parent != null && e.Parent.Id == target.Id);
    }

    /// <summary>
    /// Adds a component for the given entity.
    /// </summary>
    internal void AddComponent<T>(Entity target, T component)
        where T : Component
    {
        if (GetComponent<T>(target) != null)
            throw new InvalidOperationException();

        component.Id = Guid.NewGuid();
        component.Scene = this;
        component.Entity = target;
        components.Add(component);

        OnComponentAdded?.Invoke(this, target, component);
    }

    /// <summary>
    /// Removes a component from a given entity.
    /// </summary>
    internal bool RemoveComponent<T>(Entity target)
        where T : Component
    {
        var component = GetComponent<T>(target);

        if (component == null || !components.Remove(component))
            return false;

        OnComponentRemoved?.Invoke(this, target, component);

        return true;
    }

    internal bool RemoveComponent(Component component)
    {
        bool result = components.Remove(component);

        if (!result)
            return false;

        OnComponentRemoved?.Invoke(this, component.Entity, component);

        return true;
    }

    /// <summary>
    /// Removes all components owned by the given entity.
    /// </summary>
    internal void RemoveComponents(Entity target)
    {
        foreach (var component in GetComponents(target))
            RemoveComponent(component);
    }

    /// <summary>
    /// Retrieves the components for the given entity.
    /// </summary>
    internal IEnumerable<Component> GetComponents(Entity target)
    {
        return components.Where(c => c.Entity.Id == target.Id);
    }

    /// <summary>
    /// Retrieves the component of a certain type for the given entity.
    /// </summary>
    internal T? GetComponent<T>(Entity target)
    {
        return GetComponents(target).OfType<T>().SingleOrDefault();
    }

    public void Render()
    {
        foreach (var system in systems.OfType<IRenderable>().ToArray())
            system.Render();
    }

    public void Update(double delta)
    {
        foreach (var system in systems.OfType<IUpdateable>().ToArray())
            system.Update(delta);
    }
}
