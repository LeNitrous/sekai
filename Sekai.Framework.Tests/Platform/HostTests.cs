// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sekai.Framework.Platform;

namespace Sekai.Framework.Tests.Platform;

public class HostTests
{
    [Test]
    public void TestHostRun()
    {
        var host = new HeadlessHost();
        var runTask = Task.Factory.StartNew(() => host.Run<FrameworkGame>(), TaskCreationOptions.LongRunning);

        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(20);
        }

        Assert.That(host.IsRunning, Is.True);

        host.Exit();

        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(20);
        }

        Assert.That(host.IsRunning, Is.False);
    }

    [Test]
    public void TestHostExceptionFromMainThread()
    {
        var host = new ExceptionThrowingHost();
        var runTask = Task.Factory.StartNew(() => host.Run<ExceptionFromMainThreadGame>(), TaskCreationOptions.LongRunning);

        if (!host.Reset.WaitOne(10000))
            Assert.Fail("Failed to receive signal in time.");

        if (!host.Game.Reset.WaitOne(10000))
            Assert.Fail("Failed to receive signal in time.");

        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(20);
        }

        Assert.That(runTask.Exception?.InnerException, Is.InstanceOf<InvalidOperationException>());

        host.Dispose();
    }

    [Test]
    public void TestHostExceptionFromUpdateThread()
    {
        var host = new ExceptionThrowingHost();
        var runTask = Task.Factory.StartNew(() => host.Run<ExceptionFromUpdateThreadGame>(), TaskCreationOptions.LongRunning);

        if (!host.Reset.WaitOne(10000))
            Assert.Fail("Failed to receive signal in time.");

        if (!host.Game.Reset.WaitOne(10000))
            Assert.Fail("Failed to receive signal in time.");

        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(20);
        }

        Assert.That(runTask.Exception?.InnerException, Is.InstanceOf<InvalidOperationException>());

        host.Dispose();
    }

    private class ExceptionThrowingHost : HeadlessHost
    {
        public ExceptionThrowingGame Game { get; private set; } = null!;
        public readonly ManualResetEvent Reset = new(false);

        protected override void Initialize(Game game)
        {
            Game = (ExceptionThrowingGame)game;
            Reset.Set();
            base.Initialize(game);
        }

        protected override void Destroy()
        {
            Reset.Dispose();
            Game.Reset.Dispose();
            base.Destroy();
        }
    }

    private class ExceptionThrowingGame : Game
    {
        public readonly ManualResetEvent Reset = new(false);
    }

    private class ExceptionFromMainThreadGame : ExceptionThrowingGame
    {
        public override void Load()
        {
            Reset.Set();
            throw new InvalidOperationException();
        }
    }

    private class ExceptionFromUpdateThreadGame : ExceptionThrowingGame
    {
        public override void Update(double delta)
        {
            Reset.Set();
            throw new InvalidOperationException();
        }
    }
}
