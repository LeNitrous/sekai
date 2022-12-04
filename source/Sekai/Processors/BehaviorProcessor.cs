// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Processors;

public sealed class BehaviorProcessor : Processor<Behavior>
{
    protected override void Process(double delta, Behavior behavior)
    {
        if (behavior.HasStarted)
            behavior.Update(delta);
    }
}
