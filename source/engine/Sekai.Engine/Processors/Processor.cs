// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine.Processors;

public abstract class Processor : SceneSystem, IUpdateable
{
    public event Action<Processor, Entity> OnEntityAdded = null!;
    public event Action<Processor, Entity> OnEntityRemoved = null!;

    protected abstract Type[] Types { get; }
    private readonly List<Entity> entities = new();

    public sealed override void Initialize()
    {
        Scene.OnComponentAdded += handleComponentAdded;
        Scene.OnComponentRemoved += handleComponentRemoved;
    }

    protected abstract void Update(double delta, Entity entity);

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
            OnEntityAdded?.Invoke(this, entity);
        }
    }

    private void handleComponentRemoved(Scene scene, Entity entity, Component component)
    {
        var componentTypes = entity.GetComponents().Select(c => c.GetType());

        if (entities.Contains(entity, EqualityComparer<Entity>.Default) && !Types.All(t => componentTypes.Any(c => c.IsAssignableTo(t))))
        {
            entities.Remove(entity);
            OnEntityRemoved?.Invoke(this, entity);
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

    protected abstract void Update(double delta, Entity entity, T component);
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

    protected abstract void Update(double delta, Entity entity, T1 componentA, T2 componentB);
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

    protected abstract void Update(double delta, Entity entity, T1 componentA, T2 componentB, T3 componentC);
}

