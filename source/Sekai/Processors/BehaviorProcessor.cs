// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Processors;

internal sealed class BehaviorProcessor : Processor<Behavior>
{
    protected override void Update(Behavior component) => component.Update();
}
