// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Entities;
using Sekai.Framework.Services;
using Sekai.Framework.Systems;

namespace Sekai.Framework.Testing;

internal class TestSceneGame : Game
{
    [Resolved]
    private TestScene test { get; set; } = null!;

    [Resolved]
    private GameSystemRegistry systems { get; set; } = null!;

    private Scene? scene;
    private Entity? entity;

    public override void Load()
    {
        var sceneManager = systems.GetSystem<SceneManager>();
        scene = new Scene();

        entity = new Entity();
        entity.Add(test);

        scene.Add(entity);
        sceneManager.Load(scene);
    }

    public override void Unload()
    {
        if (entity != null)
        {
            scene?.Remove(entity);
            entity.Dispose();
        }
    }
}
