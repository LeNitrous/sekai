// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Sekai.Framework.Services;

namespace Sekai.Framework.Tests.Objects;

public class LoadableObjectTests
{
    [Test]
    public void TestLoadableCaching()
    {
        var obj = new TestLoadableObject();
        Assert.Multiple(() =>
        {
            Assert.That(() => obj.Load(), Throws.Nothing);
            Assert.That(obj.Services.Resolve<string>(true), Is.EqualTo("Hello World"));
        });
    }

    [Test]
    public void TestLoadableResolvingNullable()
    {
        var obj = new TestLoadableObject();
        Assert.Multiple(() =>
        {
            Assert.That(() => obj.Load(), Throws.Nothing);
            Assert.That(obj.Message, Is.Null);
        });
    }

    private class TestLoadableObject : LoadableObject
    {
        public new ServiceContainer Services => base.Services;

        [Cached]
        [SuppressMessage("CodeQuality", "IDE0051")]
        private readonly string message = "Hello World";

        [Resolved]
        [SuppressMessage("CodeQuality", "IDE0051")]
        [SuppressMessage("Style", "IDE0044")]
        public string? Message;
    }
}
