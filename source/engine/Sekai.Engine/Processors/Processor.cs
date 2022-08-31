// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Engine.Processors;

public abstract class Processor : SceneSystem, IUpdateable
{
    private readonly List<Entity> entities = new();

    /// <summary>
    /// Determines whether the given entity should still be eligible for processing.
    /// </summary>
    public void OnEntityUpdate(Entity entity)
    {
        if (entity.IsAlive && !entities.Contains(entity) && IsEntityValid(entity))
        {
            entities.Add(entity);
            OnEntityAdded(entity);
            return;
        }


        if (entity.IsAlive && entities.Contains(entity) && !IsEntityValid(entity))
        {
            entities.Remove(entity);
            OnEntityRemoved(entity);
            return;
        }
    }

    /// <summary>
    /// Determines whether the entity whether it is eligible for processing.
    /// </summary>
    protected virtual bool IsEntityValid(Entity entity) => entity.IsAlive;

    /// <summary>
    /// Called when an entity has been added for processing.
    /// </summary>
    protected abstract void OnEntityAdded(Entity entity);

    /// <summary>
    /// Called when an entity has been removed from processing.
    /// </summary>
    protected abstract void OnEntityRemoved(Entity entity);

    protected abstract void Update(double elapsed, Entity entity);

    public void Update(double elapsed)
    {
        foreach (var entity in entities.ToArray())
            Update(elapsed, entity);
    }
}

public abstract class Processor<T> : Processor
    where T : Component
{
    protected abstract void Update(double elapsed, Entity entity, T component);

    protected virtual void OnEntityAdded(Entity entity, T component)
    {
    }

    protected virtual void OnEntityRemoved(Entity entity, T component)
    {
    }

    protected sealed override void OnEntityAdded(Entity entity)
    {
        var component = entity.GetComponent<T>()!;
        OnEntityAdded(entity, component);
    }

    protected sealed override void OnEntityRemoved(Entity entity)
    {
        var component = entity.GetComponent<T>()!;
        OnEntityRemoved(entity, component);
    }

    protected sealed override bool IsEntityValid(Entity entity)
    {
        return base.IsEntityValid(entity) && entity.HasComponent<T>();
    }

    protected sealed override void Update(double elapsed, Entity entity)
    {
        var component = entity.GetComponent<T>()!;
        Update(elapsed, entity, component);
    }
}

public abstract class Processor<T1, T2> : Processor
    where T1 : Component
    where T2 : Component
{
    protected abstract void Update(double elapsed, Entity entity, T1 componentA, T2 componentB);

    protected virtual void OnEntityAdded(Entity entity, T1 componentA, T2 componentB)
    {
    }

    protected virtual void OnEntityRemoved(Entity entity, T1 componentA, T2 componentB)
    {
    }

    protected sealed override void OnEntityAdded(Entity entity)
    {
        var componentA = entity.GetComponent<T1>()!;
        var componentB = entity.GetComponent<T2>()!;
        OnEntityAdded(entity, componentA, componentB);
    }

    protected sealed override void OnEntityRemoved(Entity entity)
    {
        var componentA = entity.GetComponent<T1>()!;
        var componentB = entity.GetComponent<T2>()!;
        OnEntityRemoved(entity, componentA, componentB);
    }

    protected sealed override bool IsEntityValid(Entity entity)
    {
        return base.IsEntityValid(entity) && entity.HasComponent<T1>() && entity.HasComponent<T2>();
    }

    protected sealed override void Update(double elapsed, Entity entity)
    {
        var componentA = entity.GetComponent<T1>()!;
        var componentB = entity.GetComponent<T2>()!;
        Update(elapsed, entity, componentA, componentB);
    }
}

public abstract class Processor<T1, T2, T3> : Processor
    where T1 : Component
    where T2 : Component
    where T3 : Component
{
    protected abstract void Update(double elapsed, Entity entity, T1 componentA, T2 componentB, T3 componentC);

    protected virtual void OnEntityAdded(Entity entity, T1 componentA, T2 componentB, T3 componentC)
    {
    }

    protected virtual void OnEntityRemoved(Entity entity, T1 componentA, T2 componentB, T3 componentC)
    {
    }

    protected sealed override void OnEntityAdded(Entity entity)
    {
        var componentA = entity.GetComponent<T1>()!;
        var componentB = entity.GetComponent<T2>()!;
        var componentC = entity.GetComponent<T3>()!;
        OnEntityAdded(entity, componentA, componentB, componentC);
    }

    protected sealed override void OnEntityRemoved(Entity entity)
    {
        var componentA = entity.GetComponent<T1>()!;
        var componentB = entity.GetComponent<T2>()!;
        var componentC = entity.GetComponent<T3>()!;
        OnEntityRemoved(entity, componentA, componentB, componentC);
    }

    protected sealed override bool IsEntityValid(Entity entity)
    {
        return base.IsEntityValid(entity) && entity.HasComponent<T1>() && entity.HasComponent<T2>() && entity.HasComponent<T3>();
    }

    protected sealed override void Update(double elapsed, Entity entity)
    {
        var componentA = entity.GetComponent<T1>()!;
        var componentB = entity.GetComponent<T2>()!;
        var componentC = entity.GetComponent<T3>()!;
        Update(elapsed, entity, componentA, componentB, componentC);
    }
}
