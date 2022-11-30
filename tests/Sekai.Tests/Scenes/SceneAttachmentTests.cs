// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Scenes;

namespace Sekai.Tests.Scenes;

public class SceneAttachmentTests
{
    [Test]
    public void TestSceneAttach()
    {
        var scenes = new SceneCollection();
        var scene = new Scene2D();

        scenes.Add(scene);

        Assert.Multiple(() =>
        {
            Assert.That(scene.IsAttached, Is.True);
            Assert.That(scene.Root.IsAttached, Is.True);
        });

        scenes.Remove(scene);

        Assert.Multiple(() =>
        {
            Assert.That(scene.IsAttached, Is.False);
            Assert.That(scene.Root.IsAttached, Is.False);
        });
    }
}
