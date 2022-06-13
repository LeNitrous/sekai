// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Entities;

public abstract class EntityProcessor : FrameworkObject
{
    protected abstract Type ComponentType { get; }
    protected virtual Type[] RequiredComponentTypes { get; } = Type.EmptyTypes;

    public void Update(Entity entity, double elapsed)
    {
        foreach (var component in entity.GetComponents(ComponentType))
            Update(entity, component, elapsed);
    }

    protected abstract void Update(Entity entity, Component component, double elapsed);
}

public abstract class EntityProcessor<T> : EntityProcessor
    where T : Component
{
    protected sealed override Type ComponentType { get; } = typeof(T);

    protected sealed override void Update(Entity entity, Component component, double elapsed)
    {
        Update(entity, (T)component, elapsed);
    }

    protected abstract void Update(Entity entity, T component, double elapsed);
}
