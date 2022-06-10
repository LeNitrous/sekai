// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;
using Sekai.Framework.Services;

namespace Sekai.Framework.Tests.Services;

public class ServiceContainerTests
{
    [Test]
    public void TestCacheSingleton()
    {
        var services = new ServiceContainer();
        services.Cache(new TestService());

        var a = services.Resolve<TestService>(true);
        var b = services.Resolve<TestService>(true);

        Assert.That(a.ID, Is.EqualTo(b.ID));
    }

    [Test]
    public void TestCacheTransient()
    {
        var services = new ServiceContainer();
        services.Cache(() => new TestService());

        var a = services.Resolve<TestService>(true);
        var b = services.Resolve<TestService>(true);

        Assert.That(a.ID, Is.Not.EqualTo(b.ID));
    }

    [Test]
    public void TestResolveNonExistent()
    {
        var services = new ServiceContainer();
        Assert.Multiple(() =>
        {
            Assert.That(() => services.Resolve<TestService>(true), Throws.InstanceOf<ServiceNotFoundException>());
            Assert.That(() => services.Resolve<TestService>(), Throws.Nothing);
            Assert.That(() => services.Resolve<TestService>(), Is.Null);
        });
    }

    [Test]
    public void TestResolveWithParent()
    {
        var parent = new ServiceContainer();
        var parentService = new TestService();
        parent.Cache(parentService);

        var child = new ServiceContainer(parent);
        Assert.That(child.Resolve<TestService>(true).ID, Is.EqualTo(parentService.ID));

        var childService = new TestService();
        child.Cache(childService);
        Assert.That(child.Resolve<TestService>(true).ID, Is.EqualTo(childService.ID));
    }

    private class TestService
    {
        public readonly Guid ID = Guid.NewGuid();
    }
}
