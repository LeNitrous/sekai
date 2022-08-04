// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Engine;

public abstract class EntityManager : GameSystem, IUpdateable
{
    protected SceneManager SceneManager { get; private set; } = null!;
    protected abstract IReadOnlyList<Type> RequiredComponents { get; }

    protected override void Load()
    {
        SceneManager = Systems.Get<SceneManager>();
    }

    protected override void Unload()
    {
        SceneManager = null!;
    }

    protected abstract void Update(Entity entity, double elapsed);

    public void Update(double elapsed)
    {
        var scenes = SceneManager.Scenes.ToArray();

        foreach (var scene in scenes)
        {
            if (!scene.IsLoaded || !scene.Activated)
                continue;

            var entities = scene.AllAliveEntities.ToArray();

            foreach (var entity in entities)
            {
                if (!entity.IsLoaded || !entity.Activated)
                    continue;

                if (!RequiredComponents.All(c => entity.HasComponent(c)))
                    continue;

                Update(entity, elapsed);
            }
        }
    }
}
