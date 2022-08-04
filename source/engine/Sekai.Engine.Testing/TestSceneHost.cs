// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework.Internal;
using Sekai.Engine.Platform;
using Sekai.Framework.Threading;

namespace Sekai.Engine.Testing;

public static class TestSceneHost
{
    public static Host<T> SetupTest<T>(this Host<T> host, TestScene test)
        where T : Game
    {
        var context = TestExecutionContext.CurrentContext;

        host.UseWindow<HeadlessWindow>();

        host.SetupThreadController(threads =>
        {
            threads.Post(setupContext);
            threads.OnThreadAdded += setupContextForThread;
            threads.OnThreadRemoved -= setupContextForThread;
            threads.AbortOnUnobservedException = true;
        });

        host.UseLoadCallback(game =>
        {
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
            TestExecutionContext.CurrentContext.CurrentResult = context.CurrentResult;
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
