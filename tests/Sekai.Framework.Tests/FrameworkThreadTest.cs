// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading;
using NUnit.Framework;
using Sekai.Framework.Threading;

namespace Sekai.Framework.Tests;

public class FrameworkThreadTest
{
    [Test]
    public void TestThreadStartBackground()
    {
        var t = new TestThread();

        Assert.Multiple(() =>
        {
            Assert.That(() => t.Start(), Throws.Nothing);
            Assert.That(t.IsRunning, Is.True);
            Assert.That(() => t.Stop(), Throws.Nothing);
            Assert.That(t.IsRunning, Is.False);
        });

        t.Dispose();
    }

    [Test]
    public void TestThreadSendCallback()
    {
        bool flag = false;
        var t = new TestThread();
        t.Start();

        Assert.Multiple(() =>
        {
            Assert.That(() => t.Send(() => flag = true), Throws.Nothing);
            Assert.That(flag, Is.True);
        });

        t.Dispose();
    }

    [Test]
    public void TestThreadPostCallback()
    {
        bool flag = false;
        var t = new TestThread();
        t.Start();

        Assert.That(() => t.Post(() => flag = true), Throws.Nothing);

        // Give time for the thread to process the callback
        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(10);
        }

        Assert.That(flag, Is.True);

        t.Dispose();
    }

    [Test]
    public void TestThreadRaisingException()
    {
        Exception? exception = null;
        var t = new TestThread();
        t.OnUnhandledException += exceptionHandler;
        t.Start();

        Assert.That(() =>
        {
            t.Post(() =>
            {
                throw new InvalidOperationException();
            });
        }, Throws.Nothing);

        // Give time for the thread to process the callback
        for (int i = 0; i < 20; i++)
        {
            Thread.Sleep(10);
        }

        Assert.That(exception, Is.InstanceOf<InvalidOperationException>());

        t.Dispose();

        void exceptionHandler(object? sender, UnhandledExceptionEventArgs args) => exception = (Exception)args.ExceptionObject;
    }

    private class TestThread : FrameworkThread
    {
        public TestThread()
            : base("Test")
        {
        }
    }
}
