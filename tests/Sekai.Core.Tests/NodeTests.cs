// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections;
using System.Collections.Specialized;
using NUnit.Framework;

namespace Sekai.Core.Tests;

public class NodeTests
{
    [Test]
    public void Add_CanAdd_Other()
    {
        var node = new Node();
        Assert.That(() => node.Add(new Node()), Throws.Nothing);
    }

    [Test]
    public void Add_CanAdd_Other_Multiple()
    {
        var node = new Node();
        Assert.That(() => node.AddRange(new[] { new Node() }), Throws.Nothing);
    }

    [Test]
    public void Add_CannotAdd_Self()
    {
        var node = new Node();
        Assert.That(() => node.Add(node), Throws.InvalidOperationException);
    }

    [Test]
    public void Add_CannotAdd_Child()
    {
        var a = new Node();
        var b = new Node();
        a.Add(b);

        Assert.That(() => a.Add(b), Throws.InvalidOperationException);
    }

    [Test]
    public void Add_CannotAdd_Child_OfOther()
    {
        var a = new Node();
        var b = new Node();
        var c = new Node();
        b.Add(c);

        Assert.That(() => a.Add(c), Throws.InvalidOperationException);
    }

    [Test]
    public void Clear_ShouldNotFail_When_Empty()
    {
        var node = new Node();
        Assert.That(node.Clear, Throws.Nothing);
    }

    [Test]
    public void Clear_ShouldBe_Empty()
    {
        var node = new Node { new Node() };

        Assert.Multiple(() =>
        {
            Assert.That(node.Clear, Throws.Nothing);
            Assert.That(node, Is.Empty);
        });
    }

    [Test]
    public void Contains_ShouldReturn_True()
    {
        var b = new Node();
        var a = new Node { b };

        Assert.That(() => a.Contains(b), Is.True);
    }

    [Test]
    public void Contains_ShouldReturn_False()
    {
        var b = new Node();
        var a = new Node();

        Assert.That(() => a.Contains(b), Is.False);
    }

    [Test]
    public void Remove_ShouldReturn_True()
    {
        var b = new Node();
        var a = new Node { b };

        Assert.That(() => a.Remove(b), Is.True);
    }

    [Test]
    public void Remove_ShouldReturn_False()
    {
        var b = new Node();
        var a = new Node();

        Assert.That(() => a.Remove(b), Is.False);
    }

    [Test]
    public void IEnumerable_ShouldNotFail_When_Mutating()
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
    public void IEnumerable_ShouldFail_When_CollectionChangeHandled()
    {
        var node = new Node();
        node.CollectionChanged += (sender, args) => node.Add(new Node());
        Assert.That(() => node.Add(new Node()), Throws.InvalidOperationException);
    }

    [Test]
    public void CollectionChanged_ShouldRaise_Add()
    {
        var node = new Node();

        object? handledSender = null;
        NotifyCollectionChangedEventArgs? handledArgs = null;

        node.CollectionChanged += (sender, args) =>
        {
            handledArgs = args;
            handledSender = sender;
        };

        node.Add(new Node());

        Assert.Multiple(() =>
        {
            Assert.That(handledSender, Is.SameAs(node));
            Assert.That(handledArgs!.Action, Is.EqualTo(NotifyCollectionChangedAction.Add));
            Assert.That(handledArgs!.NewItems, Is.Not.Empty);
        });
    }

    [Test]
    public void CollectionChanged_ShouldRaise_Remove()
    {
        var b = new Node();
        var a = new Node { b };

        object? handledSender = null;
        NotifyCollectionChangedEventArgs? handledArgs = null;

        a.CollectionChanged += (sender, args) =>
        {
            handledArgs = args;
            handledSender = sender;
        };

        a.Remove(b);

        Assert.Multiple(() =>
        {
            Assert.That(handledSender, Is.SameAs(a));
            Assert.That(handledArgs!.Action, Is.EqualTo(NotifyCollectionChangedAction.Remove));
            Assert.That(handledArgs!.OldItems, Is.Not.Empty);
        });
    }

    [Test]
    public void CollectionChanged_ShouldRaise_Reset()
    {
        var node = new Node { new Node() };

        object? handledSender = null;
        NotifyCollectionChangedEventArgs? handledArgs = null;

        node.CollectionChanged += (sender, args) =>
        {
            handledArgs = args;
            handledSender = sender;
        };

        node.Clear();

        Assert.Multiple(() =>
        {
            Assert.That(handledSender, Is.SameAs(node));
            Assert.That(handledArgs!.Action, Is.EqualTo(NotifyCollectionChangedAction.Reset));
        });
    }

    [Test]
    public void Parent_ShouldReturn_NotNull()
    {
        var b = new Node();
        var a = new Node { b };

        Assert.That(b.Parent, Is.SameAs(a));
    }

    [Test]
    public void Parent_ShouldReturn_Null_After_Remove()
    {
        var b = new Node();
        var a = new Node { b };

        a.Remove(b);

        Assert.That(b.Parent, Is.Null);
    }

    [Test]
    public void GetRoot_ShouldReturn_Self()
    {
        var node = new Node();
        Assert.That(node.GetRoot(), Is.SameAs(node));
    }

    [Test]
    public void GetRoot_ShouldReturn_Root()
    {
        var node = new Node();
        var root = new Node { new Node { new Node { new Node { node } } } };

        Assert.That(node.GetRoot(), Is.SameAs(root));
    }
}
