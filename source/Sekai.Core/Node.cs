// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sekai.Serialization;

namespace Sekai;

/// <summary>
/// An object in a world.
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

    public int Count => nodes?.Count ?? 0;

    private List<Node>? nodes;
    private readonly object syncLock = new();

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
        if (ReferenceEquals(this, item) || Id.Equals(item.Id))
        {
            throw new InvalidOperationException("Cannot add itself as its own child.");
        }

        if (Contains(item))
        {
            throw new InvalidOperationException("Cannot add a child as its already a child of this node.");
        }

        if (item.Parent is not null)
        {
            throw new InvalidOperationException("Cannot add a node that already has a parent.");
        }

        nodes ??= new();
        nodes.Add(item);

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
        if (nodes is null || nodes.Count == 0)
        {
            return;
        }

        foreach (var item in nodes.ToArray())
        {
            Remove(item);
        }
    }

    public bool Contains(Node item)
    {
        return nodes?.Contains(item) ?? false;
    }

    public IEnumerator<Node> GetEnumerator()
    {
        return getShallowCopy().GetEnumerator();
    }

    public bool Remove(Node item)
    {
        if (nodes is null || !Contains(item))
        {
            return false;
        }

        nodes.Remove(item);
        item.Parent = null;

        return true;
    }

    private IEnumerable<Node> getShallowCopy()
    {
        if (nodes is null)
        {
            return Enumerable.Empty<Node>();
        }

        var copy = new Node[nodes.Count];

        lock (syncLock)
        {
            ((ICollection<Node>)this).CopyTo(copy, 0);
        }

        return copy;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool ICollection<Node>.IsReadOnly => false;

    void ICollection<Node>.CopyTo(Node[] array, int arrayIndex) => nodes?.CopyTo(array, arrayIndex);
}
