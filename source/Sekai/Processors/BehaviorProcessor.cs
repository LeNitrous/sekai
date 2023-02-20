// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Scenes;

namespace Sekai.Processors;

internal sealed class BehaviorProcessor : Processor<Behavior>
{
    protected override void Update(SceneCollection scenes, Behavior component) => component.Update();
}
