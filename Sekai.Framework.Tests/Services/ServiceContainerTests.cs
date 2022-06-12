// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;
using Sekai.Framework.Extensions;
using Sekai.Framework.Services;

namespace Sekai.Framework.Tests.Services;

public class ServiceContainerTests
{
    [Test]
    public void TestCacheSingleton()
    {
        var services = new ServiceContainer();
        services.Cache(new TestService());

        var a = services.Resolve<TestService>();
        var b = services.Resolve<TestService>();

        Assert.That(a.ID, Is.EqualTo(b.ID));
    }

    [Test]
    public void TestCacheTransient()
    {
        var services = new ServiceContainer();
        services.Cache(() => new TestService());

        var a = services.Resolve<TestService>();
        var b = services.Resolve<TestService>();

        Assert.That(a.ID, Is.Not.EqualTo(b.ID));
    }

    [Test]
    public void TestResolveNonExistent()
    {
        var services = new ServiceContainer();
        Assert.Multiple(() =>
        {
            Assert.That(() => services.Resolve<TestService>(), Throws.InstanceOf<ServiceNotFoundException>());
            Assert.That(() => services.Resolve<TestService>(false), Throws.Nothing);
            Assert.That(() => services.Resolve<TestService>(false), Is.Null);
        });
    }

    [Test]
    public void TestResolveWithParent()
    {
        var parent = new ServiceContainer();
        var parentService = new TestService();
        parent.Cache(parentService);

        var child = new ServiceContainer(parent);
        Assert.That(child.Resolve<TestService>().ID, Is.EqualTo(parentService.ID));

        var childService = new TestService();
        child.Cache(childService);
        Assert.That(child.Resolve<TestService>().ID, Is.EqualTo(childService.ID));
    }

    private class TestService
    {
        public readonly Guid ID = Guid.NewGuid();
    }
}
