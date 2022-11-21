// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Services;

public sealed class BehaviorService : FrameworkObject, IGameService
{
    private readonly List<Behavior> behaviors = new();

    public void Update(double delta)
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].HasStarted || !behaviors[i].IsAttached)
                continue;

            behaviors[i].Update(delta);
        }

        for (int i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].HasStarted || !behaviors[i].IsAttached)
                continue;

            behaviors[i].LateUpdate(delta);
        }
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < behaviors.Count; i++)
        {
            if (!behaviors[i].HasStarted || !behaviors[i].IsAttached)
                continue;

            behaviors[i].FixedUpdate();
        }
    }

    internal void Add(Behavior behavior)
    {
        behaviors.Add(behavior);
    }

    internal void Remove(Behavior behavior)
    {
        behaviors.Remove(behavior);
    }
}
