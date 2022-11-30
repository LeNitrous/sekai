// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;

namespace Sekai;

public abstract class Behavior : Scriptable
{
    public abstract void Update(double delta);
    protected override void OnActivate() => Scene?.Get<BehaviorProcessor>().Add(this);
    protected override void OnDeactivate() => Scene?.Get<BehaviorProcessor>().Remove(this);
}
