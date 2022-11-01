// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Sekai.Framework.Services;

namespace Sekai.Framework.Tests;

public class ServiceContainerTests
{
    [Test]
    public void TestCacheSingleton()
    {
        var instance = Guid.NewGuid();
        var container = new ServiceContainer();
        container.Register(instance);
        Assert.That(container.Resolve<Guid>(), Is.EqualTo(instance));
    }

    [Test]
    public void TestCacheTransient()
    {
        var instance = Guid.NewGuid();
        var container = new ServiceContainer();
        container.Register(() => Guid.NewGuid());
        Assert.That(container.Resolve<Guid>(), Is.Not.EqualTo(instance));
    }

    [Test]
    public void TestCacheExistingType()
    {
        var container = new ServiceContainer();
        container.Register(() => Guid.NewGuid());
        Assert.That(() => container.Register(() => Guid.NewGuid()), Throws.InvalidOperationException);
    }

    [Test]
    public void TestResolveEmptyContainer()
    {
        var container = new ServiceContainer();
        Assert.Multiple(() =>
        {
            Assert.That(container.Resolve<Guid>(false), Is.EqualTo(Guid.Empty));
            Assert.That(() => container.Resolve<Guid>(), Throws.InstanceOf<KeyNotFoundException>());
        });
    }
}
