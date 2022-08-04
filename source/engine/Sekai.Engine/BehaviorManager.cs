// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Engine;

public class BehaviorManager : EntityManager
{
    protected override IReadOnlyList<Type> RequiredComponents { get; } = new[] { typeof(Behavior) };

    protected override void Update(Entity entity, double elapsed)
    {
        var behavior = entity.GetComponent<Behavior>();
        behavior.Update(elapsed);
    }
}
