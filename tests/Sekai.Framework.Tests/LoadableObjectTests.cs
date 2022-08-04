// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sekai.Framework.Allocation;
using Sekai.Framework.Annotations;
using Sekai.Framework.Containers;

namespace Sekai.Framework.Tests;

public class LoadableObjectTests
{
    [Test]
    public void TestLoading()
    {
        var loadable = new TestLoadable();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadable.Initialize(), Throws.Nothing);
            Assert.That(() => loadable.Dispose(), Throws.Nothing);
        });
    }

    [Test]
    public void TestLoadingWithChildren()
    {
        var aLoadable = new TestLoadable();
        var bLoadable = new TestLoadable();
        Assert.Multiple(() =>
        {
            Assert.That(() => aLoadable.AddInternal(bLoadable), Throws.Nothing);
            Assert.That(() => aLoadable.Initialize(), Throws.Nothing);
            Assert.That(bLoadable.IsLoaded, Is.True);
            Assert.That(bLoadable.Parent, Is.EqualTo(aLoadable));
            Assert.That(() => aLoadable.Dispose(), Throws.Nothing);
            Assert.That(bLoadable.IsLoaded, Is.False);
            Assert.That(bLoadable.Parent, Is.Null);
            Assert.That(bLoadable.IsDisposed, Is.True);
        });
    }

    [Test]
    public void TestLoadingCaching()
    {
        const string value = "Hello World";
        var aLoadable = new TestCachingLoadable(value);
        var bLoadable = new TestCachingLoadable();
        Assert.Multiple(() =>
        {
            Assert.That(() => aLoadable.Initialize(), Throws.Nothing);
            Assert.That(aLoadable.Container.Resolve<string>(false), Is.EqualTo(value));
            Assert.That(() => bLoadable.Initialize(), Throws.InstanceOf<NullReferenceException>());
        });
    }

    [Test]
    public void TestLoadingCachingOnWriteOnlyProperty()
    {
        var loadable = new TestCachingLoadableWriteOnly();
        Assert.That(() => loadable.Initialize(), Throws.InvalidOperationException);
    }

    [Test]
    public void TestLoadingCachingSelf()
    {
        var loadable = new TestSelfCachingLoadable();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadable.Initialize(), Throws.Nothing);
            Assert.That(loadable.Container.Resolve<TestSelfCachingLoadable>(false), Is.Not.Null);
        });
    }

    [Test]
    public void TestLoadingResolving()
    {
        const string value = "Hello World";
        var aLoadable = new TestCachingLoadable(value);
        var bLoadable = new TestResolvingLoadable();
        Assert.Multiple(() =>
        {
            Assert.That(() => aLoadable.AddInternal(bLoadable), Throws.Nothing);
            Assert.That(() => aLoadable.Initialize(), Throws.Nothing);
            Assert.That(bLoadable.IsLoaded, Is.True);
            Assert.That(bLoadable.Value, Is.EqualTo(value));
        });

        var cLoadable = new TestResolvingLoadable();
        Assert.That(() => cLoadable.Initialize(), Throws.InstanceOf<KeyNotFoundException>());
    }

    [Test]
    public void TestLoadingResolvingOptional()
    {
        var loadable = new TestResolvingLoadableOptional();
        Assert.That(() => loadable.Initialize(), Throws.Nothing);
    }

    [Test]
    public void TestLoadingResolvingReadOnly()
    {
        var loadable = new TestResolvingLoadableReadOnly();
        Assert.That(() => loadable.Initialize(), Throws.InvalidOperationException);
    }

    [Test]
    public void TestLoadingInvalid()
    {
        var loadable = new TestInvalidLoadable();
        Assert.That(() => loadable.Initialize(), Throws.InvalidOperationException);
    }

    private class TestLoadable : LoadableObject
    {
        public new IReadOnlyContainer Container => base.Container;
    }

    private class TestCachingLoadable : TestLoadable
    {
        [Cached]
        public string Value = null!;
        private readonly string valueToLoad;

        public TestCachingLoadable(string value = null!)
        {
            valueToLoad = value;
        }

        protected override void Load()
        {
            Value = valueToLoad;
        }
    }

    private class TestCachingLoadableWriteOnly : TestLoadable
    {
        public string OurValue = null!;

        [Cached]
        public string Value { set => OurValue = value; }
    }

    [Cached]
    private class TestSelfCachingLoadable : TestLoadable
    {
    }

    private class TestResolvingLoadable : TestLoadable
    {
        [Resolved]
        public string Value = null!;
    }

    private class TestResolvingLoadableOptional : TestLoadable
    {
        [Resolved]
        public string? Value;
    }

    private class TestResolvingLoadableReadOnly : TestLoadable
    {
        [Resolved]
        public readonly string? Value = "Hello World";
    }

    private class TestInvalidLoadable : TestLoadable
    {
        [Cached]
        [Resolved]
        public string Value = "Hello World";
    }
}
