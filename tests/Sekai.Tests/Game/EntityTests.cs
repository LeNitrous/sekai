// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Scenes;

namespace Sekai.Tests.Game;

public class EntityTests : SekaiTestScene
{
    [Test]
    public void TestAddToScene()
    {
        var entity = new Entity();

        Scene.Add(entity);

        Assert.That(entity.IsAttached, Is.True);

        Scene.Remove(entity);

        Assert.That(entity.IsAttached, Is.False);
    }

    [Test]
    public void TestAddChild()
    {
        var parent = new Entity();
        var entity = new Entity();

        parent.Add(entity);

        Assert.That(entity.IsAttached, Is.False);

        Scene.Add(parent);

        Assert.Multiple(() =>
        {
            Assert.That(parent.IsAttached, Is.True);
            Assert.That(entity.IsAttached, Is.True);
        });

        Scene.Remove(parent);

        Assert.Multiple(() =>
        {
            Assert.That(parent.IsAttached, Is.False);
            Assert.That(entity.IsAttached, Is.False);
        });
    }

    [Test]
    public void TestAddComponent()
    {
        var entity = new Entity();

        entity.AddComponent<Transform>();

        Assert.Multiple(() =>
        {
            Assert.That(entity.HasComponent<Transform>(), Is.True);
            Assert.That(entity.GetComponent<Transform>()!.IsAttached, Is.False);
        });

        Scene.Add(entity);

        Assert.That(entity.GetComponent<Transform>()!.IsAttached, Is.True);

        Scene.Remove(entity);

        Assert.That(entity.GetComponent<Transform>()!.IsAttached, Is.False);
    }
}
