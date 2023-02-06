// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// An entity that exists in a <see cref="Scenes.Scene"/>. It can contain its own children nodes or components which can extend its functionality.
/// </summary>
[Serializable]
public sealed class Node : FrameworkObject, IReferenceable, ICollection<Node>, IReadOnlyList<Node>
{
    public Guid Id { get; private set; }

    /// <summary>
    /// The node's name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The node's URI.
    /// </summary>
    public Uri Uri { get; private set; }

    /// <summary>
    /// The node's parent.
    /// </summary>
    public Node? Parent { get; private set; }

    /// <summary>
    /// The node's scene.
    /// </summary>
    public Scene? Scene { get; internal set; }

    /// <summary>
    /// The node's components.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => new ComponentEnumerable(this, current);
        set
        {
            ClearComponents();
            AddComponentRange(value);
        }
    }

    /// <summary>
    /// The node's children.
    /// </summary>
    public IEnumerable<Node> Children
    {
        get => this;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// The node's tags.
    /// </summary>
    public IEnumerable<string> Tags
    {
        get => tags;
        set
        {
            tags.Clear();
            tags.AddRange(value);
        }
    }

    /// <summary>
    /// The number of nodes in this node.
    /// </summary>
    public int Count => nodes.Count;

    /// <summary>
    /// Gets the node at a specified index.
    /// </summary>
    /// <returns>The node at a specified index.</returns>
    public Node this[int index] => nodes[index];

    /// <summary>
    /// Invoked when a node has been added to this node
    /// </summary>
    public event EventHandler<NodeEventArgs>? OnNodeAdded;

    /// <summary>
    /// Invoked when a node has been removed from this node.
    /// </summary>
    public event EventHandler<NodeEventArgs>? OnNodeRemoved;

    /// <summary>
    /// Invoked when a component has been added to this node.
    /// </summary>
    public event EventHandler<ComponentEventArgs>? OnComponentAdded;

    /// <summary>
    /// Invoked when a component has been removed from this node.
    /// </summary>
    public event EventHandler<ComponentEventArgs>? OnComponentRemoved;

    private int current;
    private int next = 16;
    private Component[] components = Array.Empty<Component>();
    private readonly List<Node> nodes = new();
    private readonly List<string> tags = new();
    private readonly Dictionary<Type, int> indices = new();
    private static readonly string node_default_name = @"Node-";

    internal static readonly string Scheme = "node";
    private static readonly Uri node_default_uri = new(Scheme + Uri.SchemeDelimiter, UriKind.Absolute);

    public Node()
        : this(null, Guid.NewGuid())
    {
    }

    public Node(string name)
        : this(name, Guid.NewGuid())
    {
    }

    private Node(string? name, Guid guid)
    {
        Id = guid;
        Uri = node_default_uri;
        Name = name ?? node_default_name + guid;
        OnNodeAdded += handleNodeAdded;
        OnNodeRemoved += handleNodeRemoved;
    }

    /// <summary>
    /// Adds a node to this collection.
    /// </summary>
    /// <param name="item">The node to be added.</param>
    /// <returns>The node collection itself.</returns>
    /// <exception cref="InvalidOperationException">Thrown when attempting to add a node who is already present in another collection.</exception>
    public void Add(Node item)
    {
        if (item.Parent is not null || item.Scene is not null)
            throw new InvalidOperationException(@"Attempting to add a node that already has a parent.");

        nodes.Add(item);

        OnNodeAdded?.Invoke(this, new NodeEventArgs(item));
    }

    /// <summary>
    /// Adds a collection of nodes to this collection.
    /// </summary>
    /// <param name="collection">The collection of nodes to be added.</param>
    /// <returns>The node collection itself.</returns>
    public void AddRange(IEnumerable<Node> collection)
    {
        foreach (var item in collection)
            Add(item);
    }

    /// <summary>
    /// Removes all nodes from this collection.
    /// </summary>
    public void Clear()
    {
        if (nodes.Count > 0)
            RemoveRange(nodes.ToArray());
    }

    /// <summary>
    /// Gets whether a given node is present in this collection.
    /// </summary>
    /// <param name="item">The node to be searched for.</param>
    /// <returns>True if the node is present in this collection.</returns>
    public bool Contains(Node item)
    {
        return nodes.BinarySearch(item) > -1;
    }

    /// <summary>
    /// Copies the entire node collection to a compatible one-dimensional array, starting at the specified index of the target array.
    /// </summary>
    /// <param name="array">The array to be copied to.</param>
    /// <param name="arrayIndex">The starting index where copying will begin.</param>
    public void CopyTo(Node[] array, int arrayIndex)
    {
        nodes.CopyTo(array, arrayIndex);
    }

    /// <summary>
    /// Removes a node from this collection.
    /// </summary>
    /// <param name="item">The node to be removed from this collection.</param>
    /// <returns>True if the node was successfully removed.</returns>
    public bool Remove(Node item)
    {
        if (!nodes.Remove(item))
            return false;

        OnNodeRemoved?.Invoke(this, new NodeEventArgs(item));

        return true;
    }

    /// <summary>
    /// Removes a range of nodes from this collection.
    /// </summary>
    /// <param name="collection">The collection of nodes to be removed.</param>
    public void RemoveRange(IEnumerable<Node> collection)
    {
        foreach (var item in collection)
            Remove(item);
    }

    /// <summary>
    /// Gets a node using its path.
    /// </summary>
    /// <param name="path">The path to the node.</param>
    /// <returns>The node with the given path.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not attached to a scene.</exception>
    /// <exception cref="ArgumentException">Thrown when the path is invalid.</exception>
    public Node GetNode(string path)
    {
        if (!Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out var uri))
            throw new ArgumentException(@"Invalid path.", nameof(path));

        return GetNode(uri);
    }

    /// <summary>
    /// Gets a node using its URI.
    /// </summary>
    /// <param name="uri">The URI to the node.</param>
    /// <returns>The node with the given URI.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the node is not attached to a scene.</exception>
    /// <exception cref="ArgumentException">Thrown when the URI has an invalid scheme.</exception>
    public Node GetNode(Uri uri)
    {
        if (Scene is null)
            throw new InvalidOperationException(@"The node is not attached to a scene.");

        return Scene.GetNode(new Uri(Uri, uri));
    }

    /// <summary>
    /// Attempts to get a component of a given type.
    /// </summary>
    /// <param name="type">The type of component.</param>
    /// <param name="component">The component retrieved.</param>
    /// <returns>True if the component is found.</returns>
    public bool TryGetComponent(Type type, [NotNullWhen(true)] out Component? component)
    {
        if (HasComponent(type))
        {
            component = GetComponent(type);
            return true;
        }
        else
        {
            component = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to get a component of a given type.
    /// </summary>
    /// <typeparam name="T">The type of component.</typeparam>
    /// <param name="component">The component retrieved.</param>
    /// <returns>True if the component is found.</returns>
    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component)
        where T : Component
    {
        if (HasComponent<T>())
        {
            component = GetComponent<T>();
            return true;
        }
        else
        {
            component = null;
            return false;
        }
    }

    /// <summary>
    /// Gets a component of a given type.
    /// </summary>
    /// <param name="type">The component's type.</param>
    /// <returns>The component retrieved.</returns>
    /// <exception cref="ComponentNotFoundException">Thrown if the component is not found.</exception>
    public Component GetComponent(Type type)
    {
        if (!indices.TryGetValue(type, out int index))
            throw new ComponentNotFoundException($"A component of type {type} is not present in this node.");

        return components[index];
    }

    /// <summary>
    /// Gets a component of a given type.
    /// </summary>
    /// <typeparam name="T">The component's type.</typeparam>
    /// <returns>The component retrieved.</returns>
    public T GetComponent<T>()
        where T : Component
    {
        return Unsafe.As<T>(GetComponent(typeof(T)));
    }

    /// <summary>
    /// Adds a component to this node.
    /// </summary>
    /// <param name="item">The component to be added.</param>
    /// <returns>The node itself.</returns>
    /// <exception cref="ComponentExistsException">Thrown when a component of a given type already exists.</exception>
    public Node AddComponent(Component item)
    {
        var type = item.GetType();

        if (HasComponent(type))
            throw new ComponentExistsException($"A component of type {type} already exists in this node.");

        if (current + 1 > components.Length)
            Array.Resize(ref components, next >>= 1);

        indices.Add(type, current);
        components[current] = item;

        current++;
        OnComponentAdded?.Invoke(this, new ComponentEventArgs(item));

        item.Attach(this);

        return this;
    }

    /// <summary>
    /// Adds a component of a given type.
    /// </summary>
    /// <param name="type">The type of component to be added.</param>
    /// <returns>The node itself.</returns>
    /// <exception cref="InvalidCastException">Thrown when the provided type is not a component.</exception>
    /// <exception cref="TypeLoadException">Thrown when the provided type is abstract.</exception>
    /// <exception cref="ComponentExistsException">Thrown when the provided type already exists in this node.</exception>
    public Node AddComponent(Type type)
    {
        if (!type.IsSubclassOf(typeof(Component)))
            throw new InvalidCastException($"{type} must be a subclass of {nameof(Component)}.");

        if (type.IsAbstract)
            throw new TypeLoadException($"{type} must not be abstract.");

        if (HasComponent(type))
            throw new ComponentExistsException($"A component of type {type} already exists in this node.");

        return AddComponent((Component)Activator.CreateInstance(type)!);
    }

    /// <summary>
    /// Adds a component of a given type.
    /// </summary>
    /// <typeparam name="T">The type of component to be added.</typeparam>
    /// <returns>The node itself.</returns>
    /// <exception cref="InvalidCastException">Thrown when the provided type is not a component.</exception>
    /// <exception cref="TypeLoadException">Thrown when the provided type is abstract.</exception>
    /// <exception cref="ComponentExistsException">Thrown when the provided type already exists in this node.</exception>
    public Node AddComponent<T>()
        where T : Component, new()
    {
        if (HasComponent(typeof(T)))
            throw new ComponentExistsException($"A component of type {typeof(T)} already exists in this node.");

        return AddComponent(new T());
    }

    /// <summary>
    /// Adds a range of components to this node.
    /// </summary>
    /// <param name="collection">The collection of components to be added.</param>
    /// <returns>The node itself.</returns>
    public Node AddComponentRange(IEnumerable<Component> collection)
    {
        foreach (var item in collection)
            AddComponent(item);

        return this;
    }

    /// <summary>
    /// Adds a range of component types to this node.
    /// </summary>
    /// <param name="collection">The collection of component types to be added.</param>
    /// <returns>The node itself</returns>
    public Node AddComponentRange(IEnumerable<Type> collection)
    {
        foreach (var type in collection)
            AddComponent(type);

        return this;
    }

    /// <summary>
    /// Removes a component from this node.
    /// </summary>
    /// <param name="item">The component to be removed.</param>
    /// <returns>True if the component has been removed.</returns>
    public bool RemoveComponent(Component item)
    {
        var type = item.GetType();

        if (!indices.TryGetValue(type, out int index))
            return false;

        if (item != components[index])
            return false;

        var temp = components[^1];

        if (temp != item)
        {
            components[index] = temp;
            components[^1] = null!;
        }
        else
        {
            components[index] = null!;
        }

        current--;
        OnComponentRemoved?.Invoke(this, new ComponentEventArgs(item));

        item.Detach(this);

        return true;
    }

    /// <summary>
    /// Removes a component of a given type from this node.
    /// </summary>
    /// <param name="type">The component type to be removed.</param>
    /// <returns>True if the component has been removed.</returns>
    public bool RemoveComponent(Type type)
    {
        if (!TryGetComponent(type, out var component))
            return false;

        return RemoveComponent(component);
    }

    /// <summary>
    /// Removes a component of a given type from this node.
    /// </summary>
    /// <typeparam name="T">The component type to be removed.</typeparam>
    /// <returns>True if the component has been removed.</returns>
    public bool RemoveComponent<T>()
        where T : Component
    {
        return RemoveComponent(typeof(T));
    }

    /// <summary>
    /// Removes a range of components from this node.
    /// </summary>
    /// <param name="collection">The collection of nodes to be removed.</param>
    public void RemoveComponentRange(IEnumerable<Component> collection)
    {
        foreach (var item in collection)
            RemoveComponent(item);
    }

    /// <summary>
    /// Removes a range of component types from this node.
    /// </summary>
    /// <param name="collection">The collection of component types to be removed.</param>
    public void RemoveComponentRange(IEnumerable<Type> collection)
    {
        foreach (var type in collection)
            RemoveComponent(type);
    }

    /// <summary>
    /// Gets whether a component exists in this node.
    /// </summary>
    /// <param name="item">The component to be checked against.</param>
    /// <returns>True if the component is present in the node.</returns>
    public bool HasComponent(Component item)
    {
        if (!indices.TryGetValue(item.GetType(), out int index))
            return false;

        return components[index] == item;
    }

    /// <summary>
    /// Gets whether a component exists in this node.
    /// </summary>
    /// <param name="type">The component type to be checked against.</param>
    /// <returns>True if the component type is present in the node.</returns>
    public bool HasComponent(Type type)
    {
        return indices.ContainsKey(type);
    }

    /// <summary>
    /// Gets whether a component exists in this node.
    /// </summary>
    /// <typeparam name="T">The component type to be checked against.</typeparam>
    /// <returns>True if the component type is present in the node.</returns>
    public bool HasComponent<T>()
        where T : Component
    {
        return HasComponent(typeof(T));
    }

    /// <summary>
    /// Removes all components in this node.
    /// </summary>
    public void ClearComponents()
    {
        foreach (var item in components.ToArray())
        {
            if (item is not null)
                RemoveComponent(item);
        }
    }

    /// <summary>
    /// Gets the enumerator that iterates through this collection.
    /// </summary>
    public IEnumerator<Node> GetEnumerator() => nodes.GetEnumerator();

    protected sealed override void Destroy()
    {
        base.Destroy();

        Parent?.Remove(this);

        Clear();
        ClearComponents();
        
        OnNodeAdded -= handleNodeAdded;
        OnNodeRemoved -= handleNodeRemoved;
    }

    private void handleNodeAdded(object? sender, NodeEventArgs args)
    {
        args.Node.Uri = new Uri(new Uri(Uri == node_default_uri ? Uri.AbsoluteUri : Uri.AbsoluteUri + Path.AltDirectorySeparatorChar), args.Node.Name);
        args.Node.Parent = this;
    }

    private void handleNodeRemoved(object? sender, NodeEventArgs args)
    {
        args.Node.Uri = node_default_uri;
        args.Node.Parent = null;
    }

    bool ICollection<Node>.IsReadOnly => false;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private readonly struct ComponentEnumerable : IEnumerable<Component>
    {
        private readonly Node node;
        private readonly int length;

        public ComponentEnumerable(Node node, int length)
        {
            this.node = node;
            this.length = length;
        }

        public IEnumerator<Component> GetEnumerator() => new Enumerator(node, length);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private struct Enumerator : IEnumerator<Component>
        {
            public Component Current => node.components[position];

            private int position = -1;
            private readonly Node node;
            private readonly int length;

            public Enumerator(Node node, int length)
            {
                this.node = node;
                this.length = length;
            }

            public bool MoveNext()
            {
                if (length != node.current)
                    throw new InvalidOperationException(@"The node's component collection has been modified during enumeration.");

                position++;

                return node.indices.ContainsValue(position);
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
}

public class ComponentNotFoundException : Exception
{
    public ComponentNotFoundException(string message)
        : base(message)
    {
    }
}

public class ComponentExistsException : Exception
{
    public ComponentExistsException(string message)
        : base(message)
    {
    }
}
