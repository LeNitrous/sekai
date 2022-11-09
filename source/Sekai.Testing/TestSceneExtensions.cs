// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.OpenGL;
using Sekai.Scenes;
using Sekai.SDL;

namespace Sekai.Testing;

public static class TestSceneExtensions
{
    public static GameBuilder<T> Test<T>(this GameBuilder<T> builder, TestScene test)
        where T : Game, new()
    {
        return builder
            .UseGL()
            .UseSDL()
            .AddBuildAction(game =>
            {
                var scene = new Scene
                {
                    Entities = new[] { new Entity { Components = new[] { test } } }
                };

                game.Services.Resolve<SceneController>().Push(scene);
            });
    }
}
