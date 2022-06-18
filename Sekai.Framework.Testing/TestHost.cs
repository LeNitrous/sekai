// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework.Internal;
using Sekai.Framework.Entities;
using Sekai.Framework.Platform;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Testing;

/// <summary>
/// A host capable of executing a <see cref="TestScene{T}"/>
/// </summary>
internal class TestHost : HeadlessHost
{
    public readonly FrameworkThreadManager Threads;
    private readonly Component test;

    public TestHost(Component test)
    {
        this.test = test;
        Threads = new TestThreadManager();
    }

    protected sealed override void Initialize(Game game)
    {
        Threads.Post(() =>
        {
            var systems = game.Services.Resolve<GameSystemRegistry>();
            var sceneManager = systems.GetSystem<SceneManager>();

            var scene = new Scene
            {
                Entities = new[]
                {
                    new Entity { Components = new[] { test } }
                }
            };

            sceneManager.Current = scene;
        });
    }

    protected sealed override FrameworkThreadManager CreateThreadManager() => Threads;

    private class TestThreadManager : HeadlessThreadManager
    {
        private readonly TestExecutionContext context = TestExecutionContext.CurrentContext;

        public TestThreadManager()
        {
            Post(registerContext);
        }

        protected override bool OnUnobservedException(Exception exception) => true;

        protected override void OnThreadAdded(FrameworkThread thread)
        {
            thread.Post(registerContext);
        }

        private void registerContext()
        {
            TestExecutionContext.CurrentContext.CurrentResult = context.CurrentResult;
            TestExecutionContext.CurrentContext.CurrentTest = context.CurrentTest;
            TestExecutionContext.CurrentContext.CurrentCulture = context.CurrentCulture;
            TestExecutionContext.CurrentContext.CurrentPrincipal = context.CurrentPrincipal;
            TestExecutionContext.CurrentContext.CurrentRepeatCount = context.CurrentRepeatCount;
            TestExecutionContext.CurrentContext.CurrentUICulture = context.CurrentUICulture;
        }
    }
}
