// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Framework.Graphics;

namespace Sekai.Framework.Entities.Processors;

public abstract class EntityProcessor : FrameworkObject
{
    protected abstract Type ComponentType { get; }
    protected virtual Type[] RequiredComponentTypes { get; } = Type.EmptyTypes;

    public void Update(Entity entity, double elapsed)
    {
        var components = entity.GetComponents(ComponentType).ToArray();

        foreach (var component in components)
        {
            Update(entity, component, elapsed);
        }
    }

    public void Render(Entity entity, CommandList commands)
    {
        var components = entity.GetComponents(ComponentType).ToArray();

        foreach (var component in components)
        {
            Render(entity, component, commands);
        }
    }

    protected abstract void Update(Entity entity, Component component, double elapsed);
    protected abstract void Render(Entity entity, Component component, CommandList commands);
}

public abstract class EntityProcessor<T> : EntityProcessor
    where T : Component
{
    protected sealed override Type ComponentType { get; } = typeof(T);

    protected sealed override void Update(Entity entity, Component component, double elapsed)
    {
        Update(entity, (T)component, elapsed);
    }

    protected sealed override void Render(Entity entity, Component component, CommandList commands)
    {
        Render(entity, (T)component, commands);
    }

    protected abstract void Update(Entity entity, T component, double elapsed);
    protected abstract void Render(Entity entity, T component, CommandList commands);
}
