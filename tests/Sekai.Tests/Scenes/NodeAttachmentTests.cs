// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Allocation;
using Sekai.Scenes;

namespace Sekai.Tests.Scenes;

public class NodeAttachmentTests
{
    [SetUp]
    public void SetUp()
    {
        Services.Initialize();
    }

    [Test]
    public void TestNodeAttachAfterSceneAttach()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();

        var a = new Node();
        scenes.Add(scene);
        scene.Root.Add(a);

        Assert.Multiple(() =>
        {
            Assert.That(a.IsAttached, Is.True);
            Assert.That(scene.Root, Does.Contain(a));
        });

        scene.Root.Remove(a);

        Assert.Multiple(() =>
        {
            Assert.That(a.IsAttached, Is.False);
            Assert.That(scene.Root, Does.Not.Contain(a));
        });
    }

    [Test]
    public void TestNodeAttachBeforeSceneAttach()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();

        var a = new Node();
        scene.Root.Add(a);
        scenes.Add(scene);

        Assert.Multiple(() =>
        {
            Assert.That(a.IsAttached, Is.True);
            Assert.That(scene.Root, Does.Contain(a));
        });

        scenes.Remove(scene);

        Assert.Multiple(() =>
        {
            Assert.That(a.IsAttached, Is.False);
            Assert.That(scene.Root, Does.Contain(a));
        });
    }

    [Test]
    public void TestNodeDisposal()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();

        var a = new Node();
        scene.Root.Add(a);
        scenes.Add(scene);

        Assert.Multiple(() =>
        {
            Assert.That(a.IsAttached, Is.True);
            Assert.That(scene.Root, Does.Contain(a));
        });

        a.Dispose();

        Assert.Multiple(() =>
        {
            Assert.That(a.IsDisposed, Is.True);
            Assert.That(a.IsAttached, Is.False);
            Assert.That(scene.Root, Does.Not.Contain(a));
        });
    }

    [Test]
    public void TestNodeInvalidAttachment()
    {
        var scene = new Scene();
        var scenes = new SceneCollection();
        var node2D = new Node2D();
        var node3D = new Node3D();

        scenes.Add(scene);
        scene.Root.Add(node2D);

        Assert.That(() => node2D.Add(node3D), Throws.InvalidOperationException);
    }
}
