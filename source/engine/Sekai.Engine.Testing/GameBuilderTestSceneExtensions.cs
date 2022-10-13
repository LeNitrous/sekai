// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework.Internal;
using Sekai.Dummy;
using Sekai.Engine.Threading;
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
            .AddPostBuildAction(game =>
            {
                var threads = game.Services.Resolve<ThreadController>();
                threads.Window.Post(setupContext);
                threads.OnThreadAdded += setupContextForThread;
                threads.OnThreadRemoved -= setupContextForThread;
                threads.AbortOnUnobservedException = true;

                var sceneController = game.Services.Resolve<SceneController>();

                var scene = new Scene();
                scene.CreateEntity().AddComponent(test);

                sceneController.Scene = scene;
            });

        void setupContext()
        {
            TestExecutionContext.CurrentContext.CurrentResult = context!.CurrentResult;
            TestExecutionContext.CurrentContext.CurrentTest = context.CurrentTest;
            TestExecutionContext.CurrentContext.CurrentCulture = context.CurrentCulture;
            TestExecutionContext.CurrentContext.CurrentPrincipal = context.CurrentPrincipal;
            TestExecutionContext.CurrentContext.CurrentRepeatCount = context.CurrentRepeatCount;
            TestExecutionContext.CurrentContext.CurrentUICulture = context.CurrentUICulture;
        }

        void setupContextForThread(FrameworkThread thread) => thread.Post(setupContext);

        return builder;
    }
}
