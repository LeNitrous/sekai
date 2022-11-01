// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Scenes;

public class Scene : FrameworkObject
{
    /// <summary>
    /// Gets or sets the entities in this scene.
    /// </summary>
    public IEnumerable<Entity> Entities
    {
        get => entities;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets all attached entities in this scene.
    /// </summary>
    public IEnumerable<Entity> Attached => attached;

    /// <summary>
    /// The associated scene controller for this scene.
    /// </summary>
    public SceneController Controller { get; internal set; } = null!;

    /// <summary>
    /// Called when an entity has been added to the scene.
    /// </summary>
    public event Action<Scene, Entity> OnEntityAttach = null!;

    /// <summary>
    /// Called when an entity has been removed from the scene.
    /// </summary>
    public event Action<Scene, Entity> OnEntityDetach = null!;

    private readonly List<Entity> entities = new();
    private readonly List<Entity> attached = new();

    /// <summary>
    /// Adds an entity to this scene.
    /// </summary>
    public void Add(Entity entity)
    {
        if (entity.IsAttached)
            throw new InvalidOperationException(@"This entity is already attached.");

        entities.Add(entity);
        entity.Attach(this, null);
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
    /// Removes an entity from this scene.
    /// </summary>
    public void Remove(Entity entity)
    {
        if (!entities.Remove(entity))
            return;

        entity.Detach();
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
    /// Removes all entities from the scene.
    /// </summary>
    public void Clear()
    {
        RemoveRange(entities.ToArray());
    }

    /// <summary>
    /// Exits this current scene.
    /// </summary>
    public void Exit()
    {
        Controller?.Exit(this);
    }

    /// <summary>
    /// Makes this scene the current.
    /// </summary>
    public void MakeCurrent()
    {
        Controller?.MakeCurrent(this);
    }

    /// <summary>
    /// Called when the scene has been pushed as the current.
    /// </summary>
    public virtual void OnEntering(Scene last)
    {
    }

    /// <summary>
    /// Called when the scene is being exited.
    /// </summary>
    public virtual void OnExiting(Scene next)
    {
    }

    /// <summary>
    /// Called when the scene is resumed as the current.
    /// </summary>
    public virtual void OnResuming(Scene last)
    {
    }

    /// <summary>
    /// Called when the scene is suspended.
    /// </summary>
    public virtual void OnSuspending(Scene next)
    {
    }

    internal void Attach(Entity entity)
    {
        attached.Add(entity);
        OnEntityAttach?.Invoke(this, entity);
    }

    internal void Detach(Entity entity)
    {
        if (attached.Remove(entity))
            OnEntityDetach?.Invoke(this, entity);
    }
}
