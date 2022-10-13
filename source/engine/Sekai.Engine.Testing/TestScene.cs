// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Engine.Threading;

namespace Sekai.Engine.Testing;

public abstract class TestScene : Component
{
}

public abstract class TestScene<T> : TestScene
    where T : Game, new()
{
    private T game = null!;
    private Task runTask = null!;

    [OneTimeSetUp]
    public void OneTimeSetUpFromRunner()
    {
        if (!TestUtils.IsNUnit)
            return;

        game = Game.Setup<T>().SetupTest(this).Build();
        runTask = Task.Factory.StartNew(() => game.Run(), TaskCreationOptions.LongRunning);

        var threads = game.Services.Resolve<ThreadController>();

        while (!threads.IsRunning)
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
            game?.Exit();
        }
        catch
        {
        }
    }

    private void checkForErrors()
    {
        if (runTask?.Exception != null)
            throw runTask.Exception;
    }
}
