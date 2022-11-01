// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Framework.Scenes;

namespace Sekai.Framework.Tests;

public class EntityTests
{
    [Test]
    public void TestAddToScene()
    {
        var scene = new Scene();
        var entity = new Entity();

        scene.Add(entity);

        Assert.That(entity.IsAttached, Is.True);

        scene.Remove(entity);

        Assert.That(entity.IsAttached, Is.False);
    }

    [Test]
    public void TestAddChild()
    {
        var scene = new Scene();
        var parent = new Entity();
        var entity = new Entity();

        parent.Add(entity);

        Assert.That(entity.IsAttached, Is.False);

        scene.Add(parent);

        Assert.Multiple(() =>
        {
            Assert.That(parent.IsAttached, Is.True);
            Assert.That(entity.IsAttached, Is.True);
        });

        scene.Remove(parent);

        Assert.Multiple(() =>
        {
            Assert.That(parent.IsAttached, Is.False);
            Assert.That(entity.IsAttached, Is.False);
        });
    }

    [Test]
    public void TestAddComponent()
    {
        var scene = new Scene();
        var entity = new Entity();

        entity.AddComponent<Transform>();

        Assert.Multiple(() =>
        {
            Assert.That(entity.HasComponent<Transform>(), Is.True);
            Assert.That(entity.GetComponent<Transform>()!.IsAttached, Is.False);
        });

        scene.Add(entity);

        Assert.That(entity.GetComponent<Transform>()!.IsAttached, Is.True);

        scene.Remove(entity);

        Assert.That(entity.GetComponent<Transform>()!.IsAttached, Is.False);
    }
}
