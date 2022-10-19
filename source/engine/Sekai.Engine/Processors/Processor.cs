// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine.Processors;

public abstract class Processor : SceneSystem, IUpdateable
{
    protected abstract Type[] Types { get; }
    private readonly List<Entity> entities = new();

    public sealed override void Initialize()
    {
        Scene.OnComponentAdded += handleComponentAdded;
        Scene.OnComponentRemoved += handleComponentRemoved;
    }

    protected abstract void Update(double delta, Entity entity);

    protected abstract void OnEntityAdded(Entity entity);

    protected abstract void OnEntityRemoved(Entity entity);

    public void Update(double delta)
    {
        foreach (var entity in entities.ToArray())
            Update(delta, entity);
    }

    private void handleComponentAdded(Scene scene, Entity entity, Component component)
    {
        var componentTypes = entity.GetComponents().Select(c => c.GetType());

        if (!entities.Contains(entity, EqualityComparer<Entity>.Default) && Types.All(t => componentTypes.Any(c => c.IsAssignableTo(t))))
        {
            entities.Add(entity);
            OnEntityAdded(entity);
        }
    }

    private void handleComponentRemoved(Scene scene, Entity entity, Component component)
    {
        var componentTypes = entity.GetComponents().Select(c => c.GetType());

        if (entities.Contains(entity, EqualityComparer<Entity>.Default) && !Types.All(t => componentTypes.Any(c => c.IsAssignableTo(t))))
        {
            entities.Remove(entity);
            OnEntityRemoved(entity);
        }
    }

    protected override void Destroy()
    {
        Scene.OnComponentAdded -= handleComponentAdded;
        Scene.OnComponentRemoved -= handleComponentRemoved;
    }
}

public abstract class Processor<T> : Processor
    where T : Component
{
    protected sealed override Type[] Types { get; } = new[] { typeof(T) };

    protected sealed override void Update(double delta, Entity entity)
    {
        Update(delta, entity, entity.GetCommponent<T>()!);
    }

    protected sealed override void OnEntityAdded(Entity entity)
    {
        OnEntityAdded(entity, entity.GetCommponent<T>()!);
    }

    protected sealed override void OnEntityRemoved(Entity entity)
    {
        OnEntityRemoved(entity, entity.GetCommponent<T>()!);
    }

    protected abstract void Update(double delta, Entity entity, T component);

    protected abstract void OnEntityAdded(Entity entity, T component);

    protected abstract void OnEntityRemoved(Entity entity, T component);
}

public abstract class Processor<T1, T2> : Processor
    where T1 : Component
    where T2 : Component
{
    protected sealed override Type[] Types { get; } = new[] { typeof(T1), typeof(T2) };

    protected sealed override void Update(double delta, Entity entity)
    {
        Update(delta, entity, entity.GetCommponent<T1>()!, entity.GetCommponent<T2>()!);
    }

    protected sealed override void OnEntityAdded(Entity entity)
    {
        OnEntityAdded(entity, entity.GetCommponent<T1>()!, entity.GetCommponent<T2>()!);
    }

    protected sealed override void OnEntityRemoved(Entity entity)
    {
        OnEntityRemoved(entity, entity.GetCommponent<T1>()!, entity.GetCommponent<T2>()!);
    }

    protected abstract void Update(double delta, Entity entity, T1 componentA, T2 componentB);

    protected abstract void OnEntityAdded(Entity entity, T1 componentA, T2 componentB);

    protected abstract void OnEntityRemoved(Entity entity, T1 componentA, T2 componentB);
}

public abstract class Processor<T1, T2, T3> : Processor
    where T1 : Component
    where T2 : Component
    where T3 : Component
{
    protected sealed override Type[] Types { get; } = new[] { typeof(T1), typeof(T2), typeof(T3) };

    protected sealed override void Update(double delta, Entity entity)
    {
        Update(delta, entity, entity.GetCommponent<T1>()!, entity.GetCommponent<T2>()!, entity.GetCommponent<T3>()!);
    }

    protected sealed override void OnEntityAdded(Entity entity)
    {
        OnEntityAdded(entity, entity.GetCommponent<T1>()!, entity.GetCommponent<T2>()!, entity.GetCommponent<T3>()!);
    }

    protected sealed override void OnEntityRemoved(Entity entity)
    {
        OnEntityRemoved(entity, entity.GetCommponent<T1>()!, entity.GetCommponent<T2>()!, entity.GetCommponent<T3>()!);
    }

    protected abstract void Update(double delta, Entity entity, T1 componentA, T2 componentB, T3 componentC);

    protected abstract void OnEntityAdded(Entity entity, T1 componentA, T2 componentB, T3 componentC);

    protected abstract void OnEntityRemoved(Entity entity, T1 componentA, T2 componentB, T3 componentC);
}

