// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine.Processors;

public class BehaviorProcessor : Processor<Behavior>
{
    protected override void Update(double elapsed, Entity entity, Behavior component)
    {
        component.Update(elapsed);
    }
}
