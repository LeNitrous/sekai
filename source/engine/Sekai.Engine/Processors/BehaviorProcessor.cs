// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Processors;

public sealed class BehaviorProcessor : Processor<Behavior>
{
    protected override void OnEntityAdded(Entity entity, Behavior component)
    {
    }

    protected override void OnEntityRemoved(Entity entity, Behavior component)
    {
    }

    protected override void Update(double delta, Entity entity, Behavior component)
    {
        if (!component.HasStarted)
        {
            component.Start();
            component.HasStarted = true;
        }

        component.Update(delta);
    }
}
