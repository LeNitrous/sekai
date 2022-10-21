// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework.Internal;
using Sekai.Dummy;
using Sekai.Framework.Threading;

namespace Sekai.Engine.Testing;

public static class GameBuilderTestSceneExtensions
{
    public static GameBuilder<T> SetupTest<T>(this GameBuilder<T> builder, TestScene test)
        where T : Game, new()
    {
        var context = TestExecutionContext.CurrentContext;

        builder
            .UseDummy()
            .AddBuildAction(game =>
            {
                var scenes = game.Services.Resolve<SceneController>();
                var thread = game.Services.Resolve<ThreadController>();

                thread.ExecutionMode = ExecutionMode.SingleThread;

                var scene = new Scene();
                scene.CreateEntity().AddComponent(test);

                scenes.Scene = scene;
            });

        return builder;
    }
}
