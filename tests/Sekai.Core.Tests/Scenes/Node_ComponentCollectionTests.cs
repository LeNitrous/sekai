// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using NUnit.Framework;
using Sekai.Scenes;

namespace Sekai.Core.Tests.Scenes;

public class Node_ComponentCollectionTests
{
    [Test]
    public void Node_ShouldAddComponentInstance()
    {
        var node = new Node();
        var test = new TestComponent();

        Assert.That(() => node.AddComponent(test), Throws.Nothing);
    }

    [Test]
    public void Node_ShouldAddComponentInstanceRange()
    {
        var node = new Node();

        Assert.That(() => node.AddComponentRange(new[] { new TestComponent() }), Throws.Nothing);
    }

    [Test]
    public void Node_ShouldAddComponentType()
    {
        var node = new Node();

        Assert.That(() => node.AddComponent(typeof(TestComponent)), Throws.Nothing);
    }

    [Test]
    public void Node_ShouldAddComponentTypeGeneric()
    {
        var node = new Node();

        Assert.That(() => node.AddComponent<TestComponent>(), Throws.Nothing);
    }

    [Test]
    public void Node_ShouldAddComponentTypeRange()
    {
        var node = new Node();

        Assert.That(() => node.AddComponentRange(new[] { typeof(TestComponent) }), Throws.Nothing);
    }

    [Test]
    public void Node_ShouldNotAddSameComponentInstance()
    {
        var node = new Node();
        var test = new TestComponent();

        Assert.That(() =>
        {
            node.AddComponent(test);
            node.AddComponent(test);
        }, Throws.ArgumentException);
    }

    [Test]
    public void Node_ShouldNotAddSameComponentInstanceType()
    {
        var node = new Node();
        var test = new TestComponent();

        Assert.That(() =>
        {
            node.AddComponent(test);
            node.AddComponent(new TestComponent());
        }, Throws.ArgumentException);
    }

    [Test]
    public void Node_ShouldNotAddSameComponentType()
    {
        var node = new Node();

        Assert.That(() =>
        {
            node.AddComponent(typeof(TestComponent));
            node.AddComponent(typeof(TestComponent));
        }, Throws.ArgumentException);
    }

    [Test]
    public void Node_ShouldNotAddSameComponentTypeGeneric()
    {
        var node = new Node();

        Assert.That(() =>
        {
            node.AddComponent<TestComponent>();
            node.AddComponent<TestComponent>();
        }, Throws.ArgumentException);
    }

    [TestCase(typeof(string))]
    [TestCase(typeof(TestComponentNoDefaultCtor))]
    public void Node_ShouldNotAddComponentInvalidType(Type type)
    {
        var node = new Node();

        Assert.That(() => node.AddComponent(type), Throws.ArgumentException);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldNotGetComponent(bool isEmpty)
    {
        var node = new Node();

        if (!isEmpty)
        {
            node.AddComponent(new TestComponentNoDefaultCtor("Test"));
        }

        Assert.That(() => node.GetComponent(typeof(TestComponent)), Throws.InstanceOf<ComponentNotFoundException>());
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldNotGetComponentGeneric(bool isEmpty)
    {
        var node = new Node();

        if (!isEmpty)
        {
            node.AddComponent(new TestComponentNoDefaultCtor("Test"));
        }

        Assert.That(node.GetComponent<TestComponent>, Throws.InstanceOf<ComponentNotFoundException>());
    }

    [Test]
    public void Node_ShouldGetComponent()
    {
        var node = new Node();
        var test = new TestComponent();
        node.AddComponent(test);

        Assert.That(() => node.GetComponent(typeof(TestComponent)), Is.SameAs(test));
    }

    [Test]
    public void Node_ShouldGetComponentGeneric()
    {
        var node = new Node();
        var test = new TestComponent();
        node.AddComponent(test);

        Assert.That(() => node.GetComponent<TestComponent>(), Is.SameAs(test));
    }

    [Test]
    public void Node_ShouldHaveComponent()
    {
        var node = new Node();
        node.AddComponent<TestComponent>();

        Assert.That(() => node.HasComponent(typeof(TestComponent)), Is.True);
    }

    [Test]
    public void Node_ShouldHaveComponentGeneric()
    {
        var node = new Node();
        node.AddComponent<TestComponent>();

        Assert.That(node.HasComponent<TestComponent>, Is.True);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldTryGetComponent(bool hasComponent)
    {
        var node = new Node();
        var test = new TestComponent();

        if (hasComponent)
        {
            node.AddComponent(test);
        }

        Component? retrieved = null;

        Assert.Multiple(() =>
        {
            Assert.That(() => node.TryGetComponent(typeof(TestComponent), out retrieved), hasComponent ? Is.True : Is.False);
            Assert.That(retrieved, hasComponent ? Is.SameAs(test) : Is.Null);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldTryGetComponentGeneric(bool hasComponent)
    {
        var node = new Node();
        var test = new TestComponent();

        if (hasComponent)
        {
            node.AddComponent(test);
        }

        TestComponent? retrieved = null;

        Assert.Multiple(() =>
        {
            Assert.That(() => node.TryGetComponent(out retrieved), hasComponent ? Is.True : Is.False);
            Assert.That(retrieved, hasComponent ? Is.SameAs(test) : Is.Null);
        });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldGetComponentEnumerable(bool isEmpty)
    {
        var node = new Node();
        var test = new TestComponent();

        if (!isEmpty)
        {
            node.AddComponent(test);
        }

        Assert.That(node.Components, isEmpty ? Is.Empty : Is.Not.Empty);
    }

    [Test]
    public void Node_ShouldSetComponentEnumerable()
    {
        var node = new Node();
        var test = new TestComponent();

        Assert.That(() => node.Components = new Component[] { new TestComponent(), new TestComponentNoDefaultCtor("Test") }, Throws.Nothing);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldRemoveComponent(bool isEmpty)
    {
        var node = new Node();

        if (!isEmpty)
        {
            node.AddComponent<TestComponent>();

            Assert.That(() => node.RemoveComponent(typeof(TestComponent)), Is.True);
        }

        Assert.That(() => node.RemoveComponent(typeof(TestComponent)), Is.False);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void Node_ShouldRemoveComponentGeneric(bool isEmpty)
    {
        var node = new Node();

        if (!isEmpty)
        {
            node.AddComponent<TestComponent>();

            Assert.That(node.RemoveComponent<TestComponent>, Is.True);
        }

        Assert.That(node.RemoveComponent<TestComponent>, Is.False);
    }

    [Test]
    public void Node_ShouldClearComponents()
    {
        var node = new Node();
        node.AddComponent<TestComponent>();

        Assert.Multiple(() =>
        {
            Assert.That(node.ClearComponents, Throws.Nothing);
            Assert.That(node.Components, Is.Empty);
        });
    }

    private class TestComponent : Component {}

    private class TestComponentNoDefaultCtor : Component
    {
        public readonly string Name;

        public TestComponentNoDefaultCtor(string name)
        {
            Name = name;
        }
    }
}
