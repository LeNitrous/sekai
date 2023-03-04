// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections;
using NUnit.Framework;

namespace Sekai.Core.Tests;

public class NodeCollectionTests
{
    [Test]
    public void NodeShouldAddOther()
    {
        var node = new Node();

        Assert.That(() => node.Add(new Node()), Throws.Nothing);
    }

    [Test]
    public void NodeShouldNotAddSelf()
    {
        var node = new Node();

        Assert.That(() => node.Add(node), Throws.InvalidOperationException);
    }

    [Test]
    public void NodeShouldNotAddChild()
    {
        var a = new Node();
        var b = new Node();
        a.Add(b);

        Assert.That(() => a.Add(b), Throws.InvalidOperationException);
    }

    [Test]
    public void NodeShouldNotAddChildOfOther()
    {
        var a = new Node();
        var b = new Node();
        var c = new Node();
        b.Add(c);

        Assert.That(() => a.Add(c), Throws.InvalidOperationException);
    }

    [Test]
    public void NodeShouldAddRange()
    {
        var node = new Node();

        Assert.That(() => node.AddRange(new[] { new Node() }), Throws.Nothing);
    }

    [Test]
    public void NodeShouldClearChildren()
    {
        var node = new Node { new Node() };

        Assert.Multiple(() =>
        {
            Assert.That(node.Clear, Throws.Nothing);
            Assert.That(node, Is.Empty);
            Assert.That(node.Clear, Throws.Nothing);
        });
    }

    [Test]
    public void NodeShouldContainChild()
    {
        var b = new Node();
        var a = new Node { b };

        Assert.That(() => a.Contains(b), Is.True);
    }

    [Test]
    public void NodeShouldRemoveChild()
    {
        var b = new Node();
        var a = new Node { b };

        Assert.That(() => a.Remove(b), Is.True);
        Assert.That(() => a.Remove(b), Is.False);
    }

    [Test]
    public void NodeShouldHaveValidParent()
    {
        var b = new Node();
        var a = new Node { b };

        Assert.That(b.Parent, Is.SameAs(a));

        a.Remove(b);

        Assert.That(b.Parent, Is.Null);
    }

    [Test]
    public void NodeShouldNotFailEnumeration()
    {
        var node = new Node();

        Assert.That(() =>
        {
            foreach (object? obj in (IEnumerable)node)
            {
                node.Add(new Node());
            }
        }, Throws.Nothing);
    }

    [Test]
    public void NodeShouldFailMutationOnCollectionChange()
    {
        var node = new Node();
        node.CollectionChanged += (sender, args) => node.Add(new Node());
        Assert.That(() => node.Add(new Node()), Throws.InvalidOperationException);
    }
}
