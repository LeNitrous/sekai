// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Allocation;
using Sekai.Scenes;

namespace Sekai.Tests.Scenes;

public class NodeComponentTests
{
    [SetUp]
    public void SetUp()
    {
        Services.Initialize();
    }

    [Test]
    public void TestComponentAttachBeforeSceneAttach()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();

        var a = new Node();
        var b = new TestComponent();

        a.Add(b);
        scene.Root.Add(a);
        scenes.Add(scene);

        Assert.Multiple(() =>
        {
            Assert.That(b.IsAttached, Is.True);
            Assert.That(a.Components, Does.Contain(b));
        });

        a.Remove(b);

        Assert.Multiple(() =>
        {
            Assert.That(b.IsAttached, Is.False);
            Assert.That(a.Components, Does.Not.Contain(b));
        });
    }

    [Test]
    public void TestComponentAttachAfterSceneAttach()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();

        var a = new Node();
        var b = new TestComponent();

        scenes.Add(scene);
        scene.Root.Add(a);
        a.Add(b);

        Assert.Multiple(() =>
        {
            Assert.That(b.IsAttached, Is.True);
            Assert.That(a.Components, Does.Contain(b));
        });

        a.Remove(b);

        Assert.Multiple(() =>
        {
            Assert.That(b.IsAttached, Is.False);
            Assert.That(a.Components, Does.Not.Contain(b));
        });
    }

    [Test]
    public void TestComponentDisposal()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();

        var a = new Node();
        var b = new TestComponent();

        a.Add(b);
        scene.Root.Add(a);
        scenes.Add(scene);

        Assert.Multiple(() =>
        {
            Assert.That(b.IsAttached, Is.True);
            Assert.That(a.Components, Does.Contain(b));
        });

        b.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(b.IsAttached, Is.False);
            Assert.That(a.Components, Does.Not.Contain(b));
        });
    }

    private class TestComponent : Component {}
}
