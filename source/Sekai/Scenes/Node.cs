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

/// <summary>
/// An entity that exists in a <see cref="Scenes.Scene"/>. It can contain its own children nodes or components which can extend its functionality.
/// </summary>
[Serializable]
public class Node : NodeCollection, IReferenceable, IEnumerable<Component>
{
    public Guid Id { get; }

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
        get => components;
        set
        {
            ClearComponents();
            AddComponentRange(value);
        }
    }

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
    private readonly Dictionary<Type, int> indices = new();

    public Node()
    {
        OnNodeAdded += handleNodeAdded;
        OnNodeRemoved += handleNodeRemoved;
    }

    /// <summary>
    /// Gets the root node.
    /// </summary>
    /// <remarks>
    /// This is an expensive operation and should not be called frequently!
    /// </remarks>
    public Node GetRoot()
    {
        var node = this;

        while (node.Parent is not null)
        {
            node = node.Parent;
        }

        return node;
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

    protected sealed override void Destroy()
    {
        base.Destroy();

        ClearComponents();
        Parent?.Remove(this);

        OnNodeAdded -= handleNodeAdded;
        OnNodeRemoved -= handleNodeRemoved;
    }

    private void handleNodeAdded(object? sender, NodeEventArgs args) => args.Node.Parent = this;
    private void handleNodeRemoved(object? sender, NodeEventArgs args) => args.Node.Parent = null;

    IEnumerator<Component> IEnumerable<Component>.GetEnumerator() => new Enumerator(this, current);

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
