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
    public static Host<T> SetupTest<T>(this Host<T> host, TestScene test)
        where T : Game
    {
        var context = TestExecutionContext.CurrentContext;

        host.UseHeadless();

        host.UseLoadCallback(game =>
        {
            var threads = game.Container.Resolve<ThreadController>();
            threads.Post(setupContext);
            threads.OnThreadAdded += setupContextForThread;
            threads.OnThreadRemoved -= setupContextForThread;
            threads.AbortOnUnobservedException = true;

            var systems = game.Container.Resolve<GameSystemCollection>();
            var sceneManager = systems.Get<SceneManager>();
            sceneManager.Scene = new Scene
            {
                Name = test.GetType().Name,
                Entities = new[]
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

        return host;
    }
}
