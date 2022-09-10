// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Engine.Platform;

namespace Sekai.Engine.Testing;

public abstract class TestScene : Component
{
}

public abstract class TestScene<T> : TestScene
    where T : Game, new()
{
    private Host<T> host = null!;
    private Task runTask = null!;

    [OneTimeSetUp]
    public void OneTimeSetUpFromRunner()
    {
        if (!TestUtils.IsNUnit)
            return;

        host = Host.Setup<T>().SetupTest(this).Build();
        runTask = Task.Factory.StartNew(() => host.Run(), TaskCreationOptions.LongRunning);

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
            host?.Exit();
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
