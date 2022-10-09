// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Engine.Platform;
using Sekai.Headless;

namespace Sekai.Engine.Tests;

[Ignore("Dodgy behavior on action containers.")]
public class HostTests
{
    private const int wait_time = 30000;

    [Test]
    public void TestHostLifetime()
    {
        var reset = new ManualResetEvent(false);
        bool gameLoaded = false;

        var host = Host
            .Setup<TestGame>()
            .UseHeadless()
            .UseLoadCallback(game =>
            {
                gameLoaded = true;
                reset.Set();

                game.OnUnload += () =>
                {
                    gameLoaded = false;
                    reset.Set();
                };
            })
            .Build();

        var runTask = Task.Factory.StartNew(() => host.Run(), TaskCreationOptions.LongRunning);

        if (!reset.WaitOne(wait_time))
            Assert.Fail("Failed to receive signal in time.");

        Assert.Multiple(() =>
        {
            Assert.That(gameLoaded, Is.True);
            Assert.That(host.IsRunning, Is.True);
        });

        reset.Reset();
        host.Exit();

        if (!reset.WaitOne(wait_time))
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
        var reset = new ManualResetEvent(false);

        var host = Host
            .Setup<ExceptionThrowingGame>()
            .UseHeadless()
            .UseLoadCallback(_ =>  reset.Set())
            .Build();

        var runTask = Task.Factory.StartNew(() => host.Run(), TaskCreationOptions.LongRunning);

        if (!reset.WaitOne(wait_time))
            Assert.Fail("Failed to receive signal in time.");

        Assert.That(runTask.Exception?.InnerException, Is.InstanceOf<Exception>());

        host.Exit();
    }

    private class ExceptionThrowingGame : TestGame
    {
        protected override void Load()
        {
            throw new Exception();
        }
    }
}
