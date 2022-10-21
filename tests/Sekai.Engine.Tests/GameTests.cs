// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Dummy;
using Sekai.Framework.Threading;

namespace Sekai.Engine.Tests;

[Ignore("Dodgy behavior on action containers.")]
public class GameTests
{
    private const int wait_time = 30000;

    [Test]
    public void TestGameLifetime()
    {
        using var reset = new ManualResetEvent(false);
        bool gameLoaded = false;

        var game = Game
            .Setup<TestGame>()
            .UseDummy()
            .Build();

        game.OnLoaded += () => reset.Set();
        game.OnExiting += () => reset.Set();

        var threads = game.Services.Resolve<ThreadController>();
        var runTask = Task.Factory.StartNew(() => game.Run(), TaskCreationOptions.LongRunning);

        if (!reset.WaitOne(wait_time))
            Assert.Fail("Failed to receive signal in time.");

        Assert.Multiple(() =>
        {
            Assert.That(gameLoaded, Is.True);
            Assert.That(threads.IsRunning, Is.True);
        });

        reset.Reset();
        game.Exit();

        if (!reset.WaitOne(wait_time))
            Assert.Fail("Failed to receive signal in time.");

        Assert.Multiple(() =>
        {
            Assert.That(gameLoaded, Is.False);
            Assert.That(threads.IsRunning, Is.False);
        });
    }

    [Test]
    public void TestExceptionThrow()
    {
        using var reset = new ManualResetEvent(false);

        var game = Game
            .Setup<ExceptionThrowingGame>()
            .UseDummy()
            .Build();

        game.OnLoaded += () => reset.Set();

        var runTask = Task.Factory.StartNew(() => game.Run(), TaskCreationOptions.LongRunning);

        if (!reset.WaitOne(wait_time))
            Assert.Fail("Failed to receive signal in time.");

        Assert.That(runTask.Exception?.InnerException, Is.InstanceOf<Exception>());

        game.Exit();
    }

    private class ExceptionThrowingGame : TestGame
    {
        protected override void Load()
        {
            throw new Exception();
        }
    }
}
