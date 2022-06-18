// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;

namespace Sekai.Framework.Entities.Processors;

public class BehaviorProcessor : EntityProcessor<Behavior>
{
    protected override void Render(Entity entity, Behavior component, CommandList commands)
    {
    }

    protected override void Update(Entity entity, Behavior component, double elapsed)
    {
        component.Update(elapsed);
    }
}
