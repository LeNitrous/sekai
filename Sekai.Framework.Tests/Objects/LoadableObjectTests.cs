// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Framework.Services;

namespace Sekai.Framework.Tests.Objects;

public class LoadableObjectTests
{
    [Test]
    public void TestLoadableLifecycle()
    {
        var loadable = new TestLoadableWithCached();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadable.LoadInternal(), Throws.Nothing);
            Assert.That(loadable.IsLoaded, Is.True);
            Assert.That(loadable.Services.Resolve<string>(), Is.EqualTo("Hello World"));
            Assert.That(() => loadable.LoadInternal(), Throws.InvalidOperationException);
            Assert.That(() => loadable.Dispose(), Throws.Nothing);
            Assert.That(loadable.IsLoaded, Is.False);
            Assert.That(loadable.IsDisposed, Is.True);
            Assert.That(() => loadable.LoadInternal(), Throws.InvalidOperationException);
        });
    }

    [Test]
    public void TestLoadableLifecycleWithChildren()
    {
        var loadableA = new TestLoadableWithCached();
        var loadableB = new TestLoadableWithResolvable();
        loadableA.AddInternal(loadableB);
        Assert.Multiple(() =>
        {
            Assert.That(() => loadableA.LoadInternal(), Throws.Nothing);
            Assert.That(() => loadableA.InternalChildren, Is.Not.Empty);
            Assert.That(loadableA.IsLoaded, Is.True);
            Assert.That(loadableB.IsLoaded, Is.True);
            Assert.That(() => loadableB.Message, Is.EqualTo("Hello World"));
            Assert.That(() => loadableA.Dispose(), Throws.Nothing);
            Assert.That(() => loadableA.InternalChildren, Is.Empty);
            Assert.That(loadableA.IsLoaded, Is.False);
            Assert.That(loadableB.IsLoaded, Is.False);
        });
    }

    [Test]
    public void TestLoadableWithServicesOptional()
    {
        var loadable = new TestLoadableWithNullableResolvable();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadable.LoadInternal(), Throws.Nothing);
            Assert.That(loadable.IsLoaded, Is.True);
            Assert.That(loadable.Message, Is.Null);
        });
    }

    [Test]
    public void TestLoadableWithServicesRequired()
    {
        var loadable = new TestLoadableWithResolvable();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadable.LoadInternal(), Throws.InstanceOf<ServiceNotFoundException>());
            Assert.That(loadable.IsLoaded, Is.False);
            Assert.That(loadable.Message, Is.Null);
        });
    }

    [Test]
    public void TestLoadableResolvingReadOnly()
    {
        var loadableA = new TestLoadableWithResolvableReadOnlyProperty();
        Assert.That(() => loadableA.LoadInternal(), Throws.InvalidOperationException);

        var loadableB = new TestLoadableWithResolvableReadOnlyField();
        Assert.That(() => loadableB.LoadInternal(), Throws.InvalidOperationException);
    }

    [Test]
    public void TestLoadableCachingWriteOnly()
    {
        var loadable = new TestLoadableWithCachedWriteOnlyProperty();
        Assert.That(() => loadable.LoadInternal(), Throws.InvalidOperationException);
    }

    [Test]
    public void TestLoadableWithInvalidState()
    {
        var loadable = new TestLoadableWithBothResolvableAndCached();
        Assert.That(() => loadable.LoadInternal(), Throws.InvalidOperationException);
    }

    [Test]
    public void TestLoadableInitializationWithCachingAsType()
    {
        var loadable = new TestLoadableWithCachedAsType();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadable.LoadInternal(), Throws.Nothing);
            Assert.That(loadable.IsLoaded, Is.True);
            Assert.That(loadable.Services.Resolve<TestLoadable>(), Is.Not.Null);
        });
    }

    [Test]
    public void TestLoadableInitializationWithSelfCaching()
    {
        var loadableA = new TestLoadableSelfCaching();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadableA.LoadInternal(), Throws.Nothing);
            Assert.That(loadableA.IsLoaded, Is.True);
            Assert.That(loadableA.Services.Resolve<TestLoadableSelfCaching>(), Is.EqualTo(loadableA));
        });

        var loadableB = new TestLoadableSelfCachingAsType();
        Assert.Multiple(() =>
        {
            Assert.That(() => loadableB.LoadInternal(), Throws.Nothing);
            Assert.That(loadableB.IsLoaded, Is.True);
            Assert.That(loadableB.Services.Resolve<TestLoadable>(), Is.EqualTo(loadableB));
        });
    }

#pragma warning disable CS0649
#pragma warning disable CA1822

    private class TestLoadable : LoadableObject
    {
        public new ServiceContainer Services => (ServiceContainer)base.Services;
    }

    private class TestLoadableWithCached : TestLoadable
    {
        [Cached]
        public string Message = "Hello World";
    }

    private class TestLoadableWithCachedAsType : TestLoadable
    {
        [Cached(typeof(TestLoadable))]
        public TestLoadableWithCached Object = new();
    }



    private class TestLoadableWithCachedWriteOnlyProperty : TestLoadable
    {
        [Cached]
        public string Message { set { } }
    }



    private class TestLoadableWithResolvable : TestLoadable
    {
        [Resolved]
        public string Message = null!;
    }

    private class TestLoadableWithNullableResolvable : TestLoadable
    {
        [Resolved]
        public string? Message;
    }

    private class TestLoadableWithResolvableReadOnlyProperty : TestLoadable
    {
        [Resolved]
        public string Message { get; } = null!;
    }

    private class TestLoadableWithResolvableReadOnlyField : TestLoadable
    {
        [Resolved]
        public readonly string Message = null!;
    }

    private class TestLoadableWithBothResolvableAndCached : TestLoadable
    {
        [Cached]
        [Resolved]
        public readonly string Message = null!;
    }

    [Cached]
    private class TestLoadableSelfCaching : TestLoadable
    {
    }

    [Cached(typeof(TestLoadable))]
    private class TestLoadableSelfCachingAsType : TestLoadable
    {
    }

#pragma warning restore CA1822
#pragma warning restore CS0649

}
