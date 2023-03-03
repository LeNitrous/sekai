// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections;
using NUnit.Framework;

namespace Sekai.Core.Tests;

public class Node_CollectionTests
{
    [Test]
    public void Node_ShouldAddOther()
    {
        var node = new Node();

        Assert.That(() => node.Add(new Node()), Throws.Nothing);
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
        a.Add(b);

        Assert.That(() => a.Add(b), Throws.InvalidOperationException);
    }

    [Test]
    public void Node_ShouldNotAddChildOfOther()
    {
        var a = new Node();
        var b = new Node();
        var c = new Node();
        b.Add(c);

        Assert.That(() => a.Add(c), Throws.InvalidOperationException);
    }

    [Test]
    public void Node_ShouldAddRange()
    {
        var node = new Node();

        Assert.That(() => node.AddRange(new[] { new Node() }), Throws.Nothing);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldClearChildren(bool hasChild)
    {
        var node = new Node();

        if (hasChild)
        {
            node.Add(new Node());
        }

        Assert.Multiple(() =>
        {
            Assert.That(node.Clear, Throws.Nothing);
            Assert.That(node, Is.Empty);
            Assert.That(node.Clear, Throws.Nothing);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldContainChild(bool hasChild)
    {
        var a = new Node();
        var b = new Node();

        if (hasChild)
        {
            a.Add(b);
        }

        Assert.That(() => a.Contains(b), hasChild ? Is.True : Is.False);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldRemoveChild(bool hasChild)
    {
        var a = new Node();
        var b = new Node();

        if (hasChild)
        {
            a.Add(b);
        }

        Assert.That(() => a.Remove(b), hasChild ? Is.True : Is.False);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldHaveValidParent(bool hasParent)
    {
        var a = new Node();
        var b = new Node();

        if (hasParent)
        {
            a.Add(b);
        }

        Assert.That(b.Parent, hasParent ? Is.SameAs(a) : Is.Null);

        if (hasParent)
        {
            a.Remove(b);

            Assert.That(b.Parent, Is.Null);
        }
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldNotFailEnumeration(bool hasChild)
    {
        var node = new Node();

        if (hasChild)
        {
            node.Add(new Node());
        }

        Assert.That(() =>
        {
            foreach (object? obj in (IEnumerable)node)
            {
                node.Add(new Node());
            }
        }, Throws.Nothing);
    }

    [Test]
    public void Node_ShouldFailMutationOnCollectionChange()
    {
        var node = new Node();
        node.CollectionChanged += (sender, args) => node.Add(new Node());
        Assert.That(() => node.Add(new Node()), Throws.InvalidOperationException);
    }
}
