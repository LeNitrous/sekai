// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

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
            Assert.That(() => obj.Initialize(), Throws.Nothing);
            Assert.That(obj.Services.Resolve<string>(true), Is.EqualTo("Hello World"));
        });
    }

    [Test]
    public void TestLoadableResolvingNullable()
    {
        var obj = new TestLoadableObject();
        Assert.Multiple(() =>
        {
            Assert.That(() => obj.Initialize(), Throws.Nothing);
            Assert.That(obj.Message, Is.Null);
        });
    }

    private class TestLoadableObject : LoadableObject
    {
        public new ServiceContainer Services => base.Services;

#pragma warning disable IDE0051
#pragma warning disable IDE0044

        [Cached]
        private readonly string message = "Hello World";

        [Resolved]
        public string? Message;

#pragma warning restore IDE0051
#pragma warning restore IDE0044

    }
}
