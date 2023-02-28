// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// A structure within a <see cref="Scene"/> that may contain other nodes and may contain its own components.
/// </summary>
[Serializable]
[DebuggerDisplay("Count = {Count}")]
public class Node : IReferenceable, ICollection<Node>
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

    public int Count => nodes?.Count ?? 0;

    private Dictionary<Guid, Node>? nodes;

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
        if (nodes is null)
            return;

        foreach (var item in nodes.Values)
        {
            Remove(item);
        }
    }

    public bool Contains(Node item)
    {
        return nodes?.ContainsKey(item.Id) ?? false;
    }

    public void CopyTo(Node[] array, int arrayIndex)
    {
        if (nodes is null)
            return;

        nodes.Values.CopyTo(array, arrayIndex);
    }

    public IEnumerator<Node> GetEnumerator()
    {
        return Children.GetEnumerator();
    }

    public bool Remove(Node item)
    {
        if (nodes is null)
            return false;

        if (!Contains(item))
            return false;

        nodes.Remove(item.Id);
        item.Parent = null;

        return true;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    bool ICollection<Node>.IsReadOnly => false;
}
