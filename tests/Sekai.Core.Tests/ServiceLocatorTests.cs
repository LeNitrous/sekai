// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;

namespace Sekai.Core.Tests;

public class ServiceLocatorTests
{
    [Test]
    public void Add_CanAdd_Object()
    {
        var services = new ServiceLocator();
        Assert.That(() => services.Add(new object()), Throws.Nothing);
    }

    [Test]
    public void Add_CannotAdd_DuplicateType()
    {
        var services = new ServiceLocator();
        services.Add(new object());
        Assert.That(() => services.Add(new object()), Throws.ArgumentException);
    }

    [Test]
    public void Add_CannotAdd_MismatchedType()
    {
        var services = new ServiceLocator();
        Assert.That(() => services.Add(typeof(ServiceLocator), new object()), Throws.InstanceOf<InvalidCastException>());
    }

    [Test]
    public void Remove_ShouldReturn_False()
    {
        var services = new ServiceLocator();
        Assert.That(services.Remove<object>, Is.False);
    }

    [Test]
    public void Remove_ShouldReturn_True()
    {
        var services = new ServiceLocator();
        services.Add(new object());
        Assert.That(services.Remove<object>, Is.True);
    }

    [Test]
    public void Get_ShouldReturn_Null_When_NotFound_And_Required_IsFalse()
    {
        var services = new ServiceLocator();
        Assert.That(() => services.Get<object>(false), Is.Null);
    }

    [Test]
    public void Get_ShouldThrow_When_NotFound_And_Required_IsTrue()
    {
        var services = new ServiceLocator();
        Assert.That(() => services.Get<object>(), Throws.InstanceOf<ServiceNotFoundException>());
    }

    [Test]
    public void Get_ShouldReturn_Instance_When_Found()
    {
        var services = new ServiceLocator();
        object service = new();
        services.Add(service);
        Assert.That(() => services.Get<object>(), Is.SameAs(service));
    }
}
