// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Entities;
using Sekai.Framework.Logging;
using Sekai.Framework.Services;
using Sekai.Framework.Systems;

namespace Sekai.Examples.Hello;

public class ExampleGame : Game
{
    [Resolved]
    private GameSystemRegistry systems { get; set; } = null!;

    public override void Load()
    {
        var scenes = systems.GetSystem<SceneManager>();
        scenes.Current = new Scene
        {
            Entities = new[] { new Entity { Components = new[] { new MyComponent() } } }
        };
    }
}

public class MyComponent : Component
{
    public override void Load()
    {
        Logger.Log("Hello World!");
    }
}
