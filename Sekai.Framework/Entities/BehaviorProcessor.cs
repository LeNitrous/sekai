// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Entities;

public class BehaviorProcessor : EntityProcessor<Behavior>
{
    protected override void Update(Entity entity, Behavior component, double elapsed)
    {
        component.Update(elapsed);
    }
}
