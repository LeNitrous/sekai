// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Processors;

public sealed class BehaviorProcessor : Processor, IUpdateable
{
    private readonly List<Behavior> behaviors = new();

    public void Update(double delta)
    {
        foreach (var behavior in behaviors)
        {
            if (behavior.HasStarted)
                behavior.Update(delta);
        }
    }

    internal void Add(Behavior behavior)
    {
        if (!behaviors.Contains(behavior))
            behaviors.Add(behavior);
    }

    internal void Remove(Behavior behavior)
    {
        behaviors.Remove(behavior);
    }
}
