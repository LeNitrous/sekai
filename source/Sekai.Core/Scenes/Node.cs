// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// A structure within a <see cref="Scene"/> that may contain other nodes and may contain its own components.
/// </summary>
[Serializable]
[DebuggerDisplay("Count = {Count}")]
public sealed class Node : IReferenceable, ICollection<Node>
{
    public Guid Id { get; }

    /// <summary>
    /// Gets the parent of this node.
    /// </summary>
    public Node? Parent { get; private set; }

    /// <summary>
    /// Gets or sets the node's children.
    /// </summary>
    public IEnumerable<Node> Children
    {
        get => nodes is not null ? nodes.Values : Enumerable.Empty<Node>();
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    /// Gets or sets the node's components.
    /// </summary>
    public IEnumerable<Component> Components
    {
        get => components is not null ? components.Values : Enumerable.Empty<Component>();
        set
        {
            ClearComponents();
            AddComponentRange(value);
        }
    }

    public int Count => nodes?.Count ?? 0;

    private Dictionary<Guid, Node>? nodes;
    private Dictionary<Type, Component>? components;

    /// <summary>
    /// Creates a new instance of a <see cref="Node"/>.
    /// </summary>
    public Node()
        : this(Guid.NewGuid())
    {
    }

    private Node(Guid id)
    {
        Id = id;
    }

    public void Add(Node item)
    {
        if (item.Id.Equals(Id))
        {
            throw new InvalidOperationException("Cannot add itself as its own child.");
        }

        if (Contains(item))
        {
            throw new InvalidOperationException("Cannot add child as its already a child of this node.");
        }

        if (item.Parent is not null)
        {
            throw new InvalidOperationException("Cannot add node that already has a parent.");
        }

        nodes ??= new();

        nodes.Add(item.Id, item);

        item.Parent = this;
    }

    public void AddRange(IEnumerable<Node> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public void Clear()
    {
        if (nodes?.Count == 0)
        {
            return;
        }

        foreach (var item in Children.ToArray())
        {
            Remove(item);
        }
    }

    public bool Contains(Node item)
    {
        return nodes?.ContainsKey(item.Id) ?? false;
    }

    public IEnumerator<Node> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    public bool Remove(Node item)
    {
        if (nodes is null || !Contains(item))
            return false;

        nodes.Remove(item.Id);
        item.Parent = null;

        return true;
    }

    /// <summary>
    /// Adds a component to this node.
    /// </summary>
    /// <param name="component">The component to add.</param>
    /// <exception cref="ArgumentException">Thrown when the node already has a component of a given type or the component being added is already attached.</exception>
    public void AddComponent(Component component)
    {
        if (component.IsAttached)
        {
            throw new ArgumentException("The component is already attached to another node.", nameof(component));
        }

        var type = component.GetType();

        if (HasComponent(type))
        {
            throw new ArgumentException($"The node already has a component of type {type}.", nameof(component));
        }

        components ??= new();

        components.Add(type, component);

        component.Attach(this);
    }

    /// <summary>
    /// Adds a component of <paramref name="type"/> to this node.
    /// </summary>
    /// <param name="type">The component type to add.</param>
    /// <exception cref="ArgumentException">Thrown when the node already has a component of a given type, the component does not inherit <see cref="Component"/>, or if the component does not have a default parameterless constructor.</exception>
    public void AddComponent(Type type)
    {
        if (HasComponent(type))
        {
            throw new ArgumentException($"The node already has a component of type {type}.", nameof(type));
        }

        if (!type.IsSubclassOf(typeof(Component)))
        {
            throw new ArgumentException($"The type {type} is not a subclass of {nameof(Component)}.", nameof(type));
        }

        if (type.GetConstructor(Type.EmptyTypes) is null)
        {
            throw new ArgumentException($"The type {type} must have a default parameterless constructor.", nameof(type));
        }

        AddComponent((Component)Activator.CreateInstance(type)!);
    }

    /// <summary>
    /// Adds a <typeparamref name="T"/> component to this node.
    /// </summary>
    /// <typeparam name="T">The component type to add.</typeparam>
    /// <exception cref="ArgumentException">Thrown when the node already has a <typeparamref name="T"/> component.</exception>
    public void AddComponent<T>()
        where T : Component, new()
    {
        if (HasComponent<T>())
        {
            throw new ArgumentException($"The node already has a component of type {typeof(T)}.", nameof(T));
        }

        AddComponent(Activator.CreateInstance<T>());
    }

    /// <summary>
    /// Adds a range of components to this node.
    /// </summary>
    /// <param name="components">The components to add.</param>
    public void AddComponentRange(IEnumerable<Component> components)
    {
        foreach (var component in components)
        {
            AddComponent(component);
        }
    }

    /// <summary>
    /// Adds a range of components from types to this node.
    /// </summary>
    /// <param name="types">The component types to add.</param>
    public void AddComponentRange(IEnumerable<Type> types)
    {
        foreach (var type in types)
        {
            AddComponent(type);
        }
    }

    /// <summary>
    /// Gets whether this node contains a component of <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The component type to look for.</param>
    /// <returns><value>true</value> if the component contains the <paramref name="type"/>. Otherwise, returns <value>false</value>.</returns>
    public bool HasComponent(Type type)
    {
        return components?.ContainsKey(type) ?? false;
    }

    /// <summary>
    /// Gets whether this node contains a <typeparamref name="T"/> component.
    /// </summary>
    /// <typeparam name="T">The component type to look for.</typeparam>
    /// <returns><value>true</value> if the component contains a <typeparamref name="T"/> component. Otherwise, returns <value>false</value>.</returns>
    public bool HasComponent<T>()
        where T : Component
    {
        return HasComponent(typeof(T));
    }

    /// <summary>
    /// Gets the component of a given <paramref name="type"/> from this node.
    /// </summary>
    /// <param name="type">The component type to retrieve.</param>
    /// <returns>The component.</returns>
    /// <exception cref="ComponentNotFoundException">Thrown when the component of a given <paramref name="type"/> was not found.</exception>
    public Component GetComponent(Type type)
    {
        if (components is null || !HasComponent(type))
        {
            throw new ComponentNotFoundException($"The component of type {type} was not found in this node.");
        }

        return components[type];
    }

    /// <summary>
    /// Gets the <typeparamref name="T"/> component from this node.
    /// </summary>
    /// <typeparam name="T">The component type to retrieve.</typeparam>
    /// <returns>The <typeparamref name="T"/> component.</returns>
    /// <exception cref="ComponentNotFoundException">Thrown when the component of a given <typeparamref name="T"/> was not found.</exception>
    public T GetComponent<T>()
        where T : Component
    {
        var type = typeof(T);

        if (components is null || !HasComponent<T>())
        {
            throw new ComponentNotFoundException($"The component of type {type} was not found in this node.");
        }

        return Unsafe.As<T>(components[type]);
    }

    /// <summary>
    /// Gets the component of a given <paramref name="type"/> from this node.
    /// </summary>
    /// <param name="type">The component type to retrieve.</param>
    /// <param name="component">The retrieved component.</param>
    /// <returns><value>true</value> if the component contains a <typeparamref name="T"/> component. Otherwise, returns <value>false</value>.</returns>
    public bool TryGetComponent(Type type, [NotNullWhen(true)] out Component? component)
    {
        try
        {
            component = GetComponent(type);
            return true;
        }
        catch
        {
            component = null;
            return false;
        }
    }

    /// <summary>
    /// Gets the <typeparamref name="T"/> component from this node.
    /// </summary>
    /// <typeparam name="T">The component type to retrieve.</typeparam>
    /// <param name="component">The retrieved component.</param>
    /// <returns><value>true</value> if the component contains a <typeparamref name="T"/> component. Otherwise, returns <value>false</value>.</returns>
    public bool TryGetComponent<T>([NotNullWhen(true)] out T? component)
        where T : Component
    {
        try
        {
            component = GetComponent<T>();
            return true;
        }
        catch
        {
            component = null;
            return false;
        }
    }

    /// <summary>
    /// Removes a component of a given <paramref name="type"/> from this node.
    /// </summary>
    /// <param name="type">The component type to remove.</param>
    /// <returns><value>true</value> if the component was removed. Otherwise, returns <value>false</value>.</returns>
    public bool RemoveComponent(Type type)
    {
        if (components is null)
            return false;

        if (!components.Remove(type, out var component))
            return false;

        component.Detach(this);

        return true;
    }

    /// <summary>
    /// Removes the <typeparamref name="T"/> component from this node.
    /// </summary>
    /// <typeparam name="T">The component type to remove.</typeparam>
    /// <returns><value>true</value> if the component was removed. Otherwise, returns <value>false</value>.</returns>
    public bool RemoveComponent<T>()
        where T : Component
    {
        return HasComponent<T>() && RemoveComponent(typeof(T));
    }

    /// <summary>
    /// Removes all components from this node.
    /// </summary>
    public void ClearComponents()
    {
        if (components is null)
        {
            return;
        }

        foreach (var type in components.Keys.ToArray())
        {
            RemoveComponent(type);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool ICollection<Node>.IsReadOnly => false;

    void ICollection<Node>.CopyTo(Node[] array, int arrayIndex)
    {
        if (nodes is null)
            return;

        nodes.Values.CopyTo(array, arrayIndex);
    }
}
