// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework.Internal;
using Sekai.Engine.Platform;
using Sekai.Engine.Threading;
using Sekai.Framework.Threading;
using Sekai.Headless;

namespace Sekai.Engine.Testing;

public static class HostTestSceneExtensions
{
    public static HostBuilder<T> SetupTest<T>(this HostBuilder<T> builder, TestScene test)
        where T : Game, new()
    {
        var context = TestExecutionContext.CurrentContext;

        builder.UseHeadless();

        builder.UseLoadCallback(game =>
        {
            var threads = game.Container.Resolve<ThreadController>();
            threads.Post(setupContext);
            threads.OnThreadAdded += setupContextForThread;
            threads.OnThreadRemoved -= setupContextForThread;
            threads.AbortOnUnobservedException = true;

            var systems = game.Container.Resolve<SystemCollection<GameSystem>>();
            var sceneController = systems.Get<SceneController>();
            sceneController.Scene = new Scene
            {
                Name = test.GetType().Name,
                Children = new[]
                {
                    new Entity
                    {
                        Components = new[] { test }
                    }
                }
            };
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
