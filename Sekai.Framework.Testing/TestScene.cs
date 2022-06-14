// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Framework.Entities;
using Sekai.Framework.Systems;

namespace Sekai.Framework.Testing;

public abstract class TestScene : Component
{
    private TestHost? host;
    private Game? game;
    private Task? runTask;
    private Scene? scene;
    private Entity? entity;

    [OneTimeSetUp]
    public void OneTimeSetUpFromRunner()
    {
        if (!TestUtils.IsNUnit)
            return;

        host = new TestHost();
        game = CreateGame();

        host.Dispatch(() =>
        {
            var systems = game.Services.Resolve<GameSystemRegistry>();
            var sceneManager = systems.GetSystem<SceneManager>();
            scene = new Scene();

            entity = new Entity();
            entity.Add(this);

            scene.Add(entity);
            sceneManager.Load(scene);
        });

        runTask = Task.Factory.StartNew(() => host.Run(game), TaskCreationOptions.LongRunning);

        while (!IsLoaded)
        {
            checkForErrors();
            Thread.Sleep(10);
        }
    }

    [TearDown]
    public void TearDownFromRunner()
    {
        if (!TestUtils.IsNUnit)
            return;

        checkForErrors();
    }

    [OneTimeTearDown]
    public void OneTimeTearDownFromRunner()
    {
        if (!TestUtils.IsNUnit)
            return;

        try
        {
            game?.Dispose();
            host?.Dispose();
        }
        catch
        {
        }
    }

    protected abstract Game CreateGame();

    private void checkForErrors()
    {
        if (runTask?.Exception != null)
            throw runTask.Exception;
    }
}
