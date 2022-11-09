// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;

namespace Sekai.Testing;

public abstract class TestScene : Component
{
}

public abstract class TestScene<T> : TestScene
    where T : Game, new()
{
    private T game = null!;
    private Task task = null!;

    [OneTimeSetUp]
    public void OneTimeSetUpFromRunner()
    {
        if (!TestUtils.IsNUnit)
            return;

        Environment.SetEnvironmentVariable("SEKAI_HEADLESS_TEST", "true", EnvironmentVariableTarget.Process);

        game = Game.Setup<T>().Test(this).Build();
        task = Task.Factory.StartNew(() => game.Run(), TaskCreationOptions.LongRunning);

        while (!IsAttached)
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
        if (task?.Exception != null)
            throw task.Exception;
    }
}
