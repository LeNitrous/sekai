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
        var host = new HeadlessHost();
        var runTask = Task.Factory.StartNew(() => host.Run<ExceptionFromMainThreadGame>(), TaskCreationOptions.LongRunning);

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
        var host = new HeadlessHost();
        var runTask = Task.Factory.StartNew(() => host.Run<ExceptionFromUpdateThreadGame>(), TaskCreationOptions.LongRunning);

        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(20);
        }

        Assert.That(runTask.Exception?.InnerException, Is.InstanceOf<InvalidOperationException>());

        host.Dispose();
    }

    private class ExceptionFromMainThreadGame : Game
    {
        public override void Load()
        {
            throw new InvalidOperationException();
        }
    }

    private class ExceptionFromUpdateThreadGame : Game
    {
        public override void Update(double delta)
        {
            throw new InvalidOperationException();
        }
    }
}
