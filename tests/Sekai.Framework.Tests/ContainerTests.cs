// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sekai.Framework.Containers;

namespace Sekai.Framework.Tests;

public class ContainerTests
{
    [Test]
    public void TestCacheSingleton()
    {
        var instance = Guid.NewGuid();
        var container = new Container();
        container.Cache(instance);
        Assert.That(container.Resolve<Guid>(), Is.EqualTo(instance));
    }

    [Test]
    public void TestCacheTransient()
    {
        var instance = Guid.NewGuid();
        var container = new Container();
        container.Cache(() => Guid.NewGuid());
        Assert.That(container.Resolve<Guid>(), Is.Not.EqualTo(instance));
    }

    [Test]
    public void TestCacheExistingType()
    {
        var container = new Container();
        container.Cache(() => Guid.NewGuid());
        Assert.That(() => container.Cache(() => Guid.NewGuid()), Throws.InvalidOperationException);
    }

    [Test]
    public void TestResolveEmptyContainer()
    {
        var container = new Container();
        Assert.Multiple(() =>
        {
            Assert.That(container.Resolve<Guid>(false), Is.EqualTo(Guid.Empty));
            Assert.That(() => container.Resolve<Guid>(), Throws.InstanceOf<KeyNotFoundException>());
        });
    }

    [Test]
    public void TestResolveFromParent()
    {
        var parent = new Container();
        var child = new Container { Parent = parent };
        parent.Cache("Hello World");
        Assert.That(child.Resolve<string>(), Is.EqualTo("Hello World"));
    }
}
