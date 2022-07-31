// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sekai.Framework.Logging;

namespace Sekai.Framework.Tests;

public class LoggerTest
{
    [Test]
    public void TestLogMessage()
    {
        var listener = new TestListener();
        var logger = (ILogger)Logger.GetLogger("test");
        listener.Level = LogLevel.Debug;
        logger.OnMessageLogged += listener;

        Assert.Multiple(() =>
        {
            Assert.That(() => logger.Log("Hello World"), Throws.Nothing);
            Assert.That(listener.MessagesLogged, Is.EqualTo(1));
            Assert.That(listener.Messages.First(), Is.EqualTo("Hello World"));
        });
    }

    [Test]
    public void TestLogMessageLevelFiltering()
    {
        var listener = new TestListener();
        var logger = (ILogger)Logger.GetLogger("test");
        listener.Level = LogLevel.Error;
        logger.OnMessageLogged += listener;

        Assert.Multiple(() =>
        {
            Assert.That(() => logger.Log("Hello World"), Throws.Nothing);
            Assert.That(listener.MessagesLogged, Is.EqualTo(0));
            Assert.That(listener.Messages, Is.Empty);
        });
    }

    [Test]
    public void TestLogMessageThrottling()
    {
        var listener = new TestListener();
        var logger = (ILogger)Logger.GetLogger("test");
        listener.LogEveryCount = 5;
        logger.OnMessageLogged += listener;

        logger.Log("Hello World");
        Assert.That(listener.Flushed, Is.Empty);

        logger.Log("Hello World");
        logger.Log("Hello World");
        logger.Log("Hello World");
        logger.Log("Hello World");
        Assert.That(listener.Flushed, Has.Count.EqualTo(5));
    }

    private class TestListener : LogListener
    {
        public readonly List<string> Messages = new();
        public readonly List<string> Flushed = new();

        protected override void OnNewMessage(string message)
        {
            Messages.Add(message);
        }

        protected override void Flush()
        {
            Flushed.AddRange(Messages);
        }
    }
}
