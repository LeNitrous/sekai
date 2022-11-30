// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Sekai.Serialization;

namespace Sekai.Scenes;

public class Node : AttachableObject, IReferenceable, ICollection<Node>, IReadOnlyList<Node>, IEnumerable<Component>
{
    public Guid Id { get; }

    /// <summary>
    /// The scene where this node is attached to.
    /// </summary>
    public Scene? Scene { get; private set; }

    /// <summary>
    /// The parent where this node is attached to.
    /// </summary>
    public Node? Parent { get; private set; }

    /// <summary>
    /// Gets a child node at a given index.
    /// </summary>
    public Node this[int index] => children[index];

    /// <summary>
    /// The node's children.
    /// </summary>
    public IEnumerable<Node> Children
    {
        get => children;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// The number of children this node has.
    /// </summary>
    public int Count => children.Count;

    /// <summary>
    /// Whether this node is read-only.
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// The node's components.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => components;
        set
        {
            RemoveRange(components.ToArray());
            AddRange(value);
        }
    }

    private int next = 16;
    private int current;
    private Component[] components = Array.Empty<Component>();
    private readonly Dictionary<Type, int> indices = new();
    private readonly List<Node> children = new();

    /// <summary>
    /// Adds a child node to this node.
    /// </summary>
    public void Add(Node item)
    {
        if (item == this)
            throw new InvalidOperationException(@"Cannot add self as a child.");

        if (item.IsAttached)
            throw new InvalidOperationException(@"Node is already attached.");

        if (children.Contains(item))
            return;

        children.Add(item);

        if (IsAttached)
            item.Attach(this);
    }

    /// <summary>
    /// Adss a range of children nodes.
    /// </summary>
    public void AddRange(IEnumerable<Node> items)
    {
        foreach (var item in items)
            Add(item);
    }

    /// <summary>
    /// Removes all children from this node.
    /// </summary>
    public void Clear()
    {
        foreach (var item in children)
            Remove(item);
    }

    /// <summary>
    /// Returns whether a node is a child of this node.
    /// </summary>
    public bool Contains(Node item)
    {
        return children.Contains(item);
    }

    /// <summary>
    /// Copies the children to another array.
    /// </summary>
    public void CopyTo(Node[] array, int arrayIndex)
    {
        children.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Returns the child enumerator for this node.
    /// </summary>
    public IEnumerator<Node> GetEnumerator()
    {
        return children.GetEnumerator();
    }

    /// <summary>
    /// Removes a child node from this node.
    /// </summary>
    public bool Remove(Node item)
    {
        if (item == this)
            throw new InvalidOperationException(@"Cannot remove self from children.");

        if (!item.IsAttached)
            return false;

        if (!children.Contains(item))
            return false;

        children.Remove(item);

        if (IsAttached)
            item.Detach(this);

        return true;
    }

    /// <summary>
    /// Removes a range of children nodes from this node.
    /// </summary>
    public void RemoveRange(IEnumerable<Node> items)
    {
        foreach (var item in items)
            Remove(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Adds a component of a given type to this node.
    /// </summary>
    public void Add<U>()
        where U : Component, new()
    {
        if (indices.ContainsKey(typeof(U)))
            throw new InvalidOperationException($"A component of type {typeof(U)} already exists in this collection.");

        Add(Activator.CreateInstance<U>());
    }

    /// <summary>
    /// Adds a component to this node.
    /// </summary>
    public void Add(Component item)
    {
        var type = item.GetType();

        if (Contains(item))
            throw new InvalidOperationException(@"The component already exists in this collection.");

        if (indices.ContainsKey(type))
            throw new InvalidOperationException($"A component of type {type} already exists in this collection.");

        if (current + 1 > components.Length)
            Array.Resize(ref components, next >>= 1);

        indices.Add(type, current);
        components[current] = item;

        current++;

        if (IsAttached)
            item.Attach(this);
    }

    /// <summary>
    /// Adds a range of components to this node.
    /// </summary>
    public void AddRange(IEnumerable<Component> range)
    {
        foreach (var component in range)
            Add(component);
    }

    /// <summary>
    /// Removes a component of a given type from this node.
    /// </summary>
    public bool Remove<U>()
        where U : Component, new()
    {
        if (!TryGet<U>(out var component))
            return false;

        return Remove(component);
    }

    /// <summary>
    /// Removes the component from this node.
    /// </summary>
    public bool Remove(Component item)
    {
        if (!indices.TryGetValue(components.GetType(), out int index))
            return false;

        var temp = components[^1];
        components[index] = temp;
        components[^1] = null!;

        if (temp is not null && temp != item)
            indices[temp.GetType()] = index;

        current--;

        if (IsAttached)
            item.Detach(this);

        return true;
    }

    /// <summary>
    /// Removes a range of components from this node.
    /// </summary>
    public void RemoveRange(IEnumerable<Component> range)
    {
        foreach (var component in range)
            Remove(component);
    }

    /// <summary>
    /// Returns whether a component of a given type exists on this node.
    /// </summary>
    public bool Has<U>()
        where U : Component, new()
    {
        return indices.ContainsKey(typeof(U));
    }

    /// <summary>
    /// Returns whether the component exists on this node.
    /// </summary>
    public bool Contains(Component component)
    {
        return components.Contains(component);
    }

    /// <summary>
    /// Returns the component of a given type from this node.
    /// </summary>
    public U Get<U>()
        where U : Component, new()
    {
        if (!indices.TryGetValue(typeof(U), out int index))
            throw new KeyNotFoundException();

        return Unsafe.As<U>(components[index]);
    }

    /// <summary>
    /// Returns the component of a given type from this node.
    /// </summary>
    public bool TryGet<U>([NotNullWhen(true)] out U? component)
        where U : Component, new()
    {
        if (!Has<U>())
        {
            component = null;
            return false;
        }

        component = Get<U>();
        return true;
    }

    protected override void OnAttach()
    {
        foreach (var component in components)
            component.Attach(this);

        foreach (var child in children)
            child.Attach(this);
    }

    protected override void OnDetach()
    {
        foreach (var component in components)
            component.Detach(this);

        foreach (var child in children)
            child.Detach(this);
    }

    protected sealed override void Destroy() => Parent?.Remove(this);

    internal void Attach(Scene scene)
    {
        if (IsAttached)
            return;

        if (!CanAttach(scene))
            throw new InvalidOperationException();

        Scene = scene;

        Attach();
    }

    internal void Detach(Scene scene)
    {
        if (!IsAttached)
            return;

        if (Scene != scene)
            throw new InvalidOperationException();

        Scene = null;

        Detach();
    }

    internal void Attach(Node node)
    {
        if (IsAttached)
            return;

        if (!CanAttach(node))
            throw new InvalidOperationException();

        Scene = node.Scene;
        Parent = node;

        Attach();
    }

    internal void Detach(Node node)
    {
        if (!IsAttached)
            return;

        if (Parent != node)
            throw new InvalidOperationException();

        Scene = null;
        Parent = null;

        Detach();
    }

    internal virtual bool CanAttach(Scene scene) => scene.GetType().Equals(typeof(Scene));
    internal virtual bool CanAttach(Node parent) => parent.GetType().Equals(typeof(Node));

    IEnumerator<Component> IEnumerable<Component>.GetEnumerator() => new Enumerator(this);

    private struct Enumerator : IEnumerator<Component>
    {
        public Component Current => node.components[position];

        private int position = -1;
        private readonly Node node;

        public Enumerator(Node node)
        {
            this.node = node;
        }

        public bool MoveNext()
        {
            return node.indices.ContainsValue(position++);
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current => Current;
    }
}
