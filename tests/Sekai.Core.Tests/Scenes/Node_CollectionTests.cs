// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Scenes;

namespace Sekai.Core.Tests.Scenes;

public class Node_CollectionTests
{
    [Test]
    public void Node_ShouldAddOther()
    {
        var a = new Node();
        var b = new Node();

        Assert.Multiple(() =>
        {
            Assert.That(() => a.Add(b), Throws.Nothing);
            Assert.That(a, Does.Contain(b));
            Assert.That(b.Parent, Is.EqualTo(a));
        });
    }

    [Test]
    public void Node_ShouldAddRange()
    {
        var node = new Node();

        Assert.Multiple(() =>
        {
            Assert.That(node, Has.Count.Zero);
            Assert.That(node.Children, Is.Empty);
            Assert.That(() => node.Children = new[] { new Node(), new Node(), new Node() }, Throws.Nothing);
            Assert.That(node, Has.Count.Not.Zero);
            Assert.That(node.Children, Is.Not.Empty);
        });
    }

    [Test]
    public void Node_ShouldClearChildren()
    {
        var node = new Node();

        Assert.Multiple(() =>
        {
            Assert.That(node.Clear, Throws.Nothing);
            Assert.That(() => node.Add(new Node()), Throws.Nothing);
            Assert.That(node, Is.Not.Empty);
        });

        Assert.Multiple(() =>
        {
            Assert.That(node.Clear, Throws.Nothing);
            Assert.That(node, Is.Empty);
        });
    }

    [Test]
    public void Node_ShouldNotAddSelf()
    {
        var node = new Node();
        Assert.That(() => node.Add(node), Throws.InvalidOperationException);
    }

    [Test]
    public void Node_ShouldNotAddChild()
    {
        var a = new Node();
        var b = new Node();

        Assert.That(() => a.Add(b), Throws.Nothing);
        Assert.That(() => a.Add(b), Throws.InvalidOperationException);

        var c = new Node();
        Assert.That(() => c.Add(b), Throws.InvalidOperationException);
    }

    [Test]
    public void Node_ShouldRemoveOther()
    {
        var a = new Node();
        var b = new Node();

        Assert.Multiple(() =>
        {
            Assert.That(() => a.Remove(b), Is.False);
            Assert.That(() => a.Add(b), Throws.Nothing);
            Assert.That(() => a.Remove(b), Is.True);
            Assert.That(b.Parent, Is.Null);
        });

        Assert.That(() => a.Remove(b), Is.False);
    }
}
