// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework.Internal;
using Sekai.Framework.Entities;
using Sekai.Framework.Platform;
using Sekai.Framework.Systems;
using Sekai.Framework.Threading;
using Silk.NET.Windowing;

namespace Sekai.Framework.Testing;

/// <summary>
/// A host capable of executing a <see cref="TestScene{T}"/>
/// </summary>
internal class TestHost : ViewHost
{
    public readonly FrameworkThreadManager Threads;
    private readonly Component test;

    public TestHost(Component test)
    {
        Threads = new TestThreadManager();

        if (View is IWindow window)
            window.IsVisible = false;

        this.test = test;
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

    private class TestThreadManager : FrameworkThreadManager
    {
        private readonly TestExecutionContext context = TestExecutionContext.CurrentContext;

        public TestThreadManager()
        {
            Post(registerContext);
        }

        protected override MainThread CreateMainThread() => new();
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
