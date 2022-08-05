// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Engine.Platform;
using Sekai.Engine.Testing;

namespace Sekai.Engine.Tests;

public class HostTests
{
    [Test]
    public void TestHostLifetime()
    {
        var reset = new ManualResetEvent(false);
        bool gameLoaded = false;

        var host = Host
            .Setup<TestGame>()
            .UseView<HeadlessView>()
            .UseLoadCallback(game =>
            {
                gameLoaded = true;
                reset.Set();

                game.OnUnload += () =>
                {
                    gameLoaded = false;
                    reset.Set();
                };
            });

        var runTask = Task.Factory.StartNew(() => host.Run(), TaskCreationOptions.LongRunning);

        if (!reset.WaitOne(10000))
            Assert.Fail("Failed to receive signal in time.");

        Assert.Multiple(() =>
        {
            Assert.That(gameLoaded, Is.True);
            Assert.That(host.IsRunning, Is.True);
        });

        reset.Reset();
        host.Exit();

        if (!reset.WaitOne(10000))
            Assert.Fail("Failed to receive signal in time.");

        Assert.Multiple(() =>
        {
            Assert.That(gameLoaded, Is.False);
            Assert.That(host.IsDisposed, Is.True);
        });

        reset.Dispose();
    }

    [Test]
    public void TestExceptionThrow()
    {
        var host = Host
            .Setup<ExceptionThrowingGame>()
            .UseView<HeadlessView>();

        var runTask = Task.Factory.StartNew(() => host.Run(), TaskCreationOptions.LongRunning);

        Thread.Sleep(100);

        Assert.That(runTask.Exception?.InnerException, Is.InstanceOf<Exception>());

        host.Dispose();
    }

    private class ExceptionThrowingGame : TestGame
    {
        protected override void Load()
        {
            throw new Exception();
        }
    }
}
