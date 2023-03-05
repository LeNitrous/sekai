// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Sekai.Serialization;

namespace Sekai;

/// <summary>
/// An object in a world.
/// </summary>
[Serializable]
[DebuggerDisplay("Name = {Name}, Count = {Count}")]
public class Node : IReferenceable, ICollection<Node>, INotifyCollectionChanged
{
    public Guid Id { get; }

    /// <summary>
    /// Gets the parent of this node.
    /// </summary>
    public Node? Parent { get; private set; }

    /// <summary>
    /// Gets the name of this node.
    /// </summary>
    public string Name { get; }

    public int Count => nodes.Count;

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Gets the depth of this node relative to the root.
    /// </summary>
    public int Depth { get; private set; }

    private int entrants;
    private readonly object syncLock = new();
    private readonly List<Node> nodes = new();

    /// <summary>
    /// Creates a new instance of a <see cref="Node"/>.
    /// </summary>
    /// <param name="name">The node's name.</param>
    public Node(string? name = null)
        : this(Guid.NewGuid(), name)
    {
    }

    private Node(Guid id, string? name)
    {
        Id = id;
        Name = name ?? id.ToString();
    }

    /// <summary>
    /// Gets the root node.
    /// </summary>
    /// <remarks>
    /// This operation is expensive and must not be called frequently. Refer to caching the returned value a when needed often.
    /// </remarks>
    /// <returns>The root node.</returns>
    public Node GetRoot()
    {
        var node = this;

        while (node.Parent is not null)
        {
            node = node.Parent;
        }

        return node;
    }

    public void Add(Node item)
    {
        add(item);

        raiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, nodes!.Count - 1));
    }

    public void AddRange(IEnumerable<Node> items)
    {
        foreach (var item in items)
        {
            add(item);
        }

        raiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToList()));
    }

    public void Clear()
    {
        if (nodes.Count == 0)
        {
            return;
        }

        foreach (var item in nodes.ToList())
        {
            remove(item);
        }

        raiseCollectionChanged(reset_event_args);
    }

    public bool Contains(Node item)
    {
        return nodes.Contains(item);
    }

    public IEnumerator<Node> GetEnumerator()
    {
        return getShallowCopy().GetEnumerator();
    }

    public bool Remove(Node item)
    {
        if (!remove(item))
        {
            return false;
        }

        raiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));

        return true;
    }

    private void add(Node item)
    {
        ensureMutationsAllowed();

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

        nodes.Add(item);

        item.Depth = Depth + 1;
        item.Parent = this;
    }

    private bool remove(Node item)
    {
        ensureMutationsAllowed();

        if (!Contains(item))
        {
            return false;
        }

        nodes.Remove(item);

        item.Depth = 0;
        item.Parent = null;

        return true;
    }

    private void ensureMutationsAllowed()
    {
        if (entrants > 0)
        {
            throw new InvalidOperationException($"Cannot mutate collection while in a {nameof(CollectionChanged)} event.");
        }
    }

    private void raiseCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        if (CollectionChanged is not null)
        {
            entrants++;

            try
            {
                CollectionChanged(this, args);
            }
            finally
            {
                entrants--;
            }
        }
    }

    private IEnumerable<Node> getShallowCopy()
    {
        var copy = new Node[nodes.Count];

        lock (syncLock)
        {
            ((ICollection<Node>)this).CopyTo(copy, 0);
        }

        return copy;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    bool ICollection<Node>.IsReadOnly => false;

    void ICollection<Node>.CopyTo(Node[] array, int arrayIndex) => nodes.CopyTo(array, arrayIndex);

    private static readonly NotifyCollectionChangedEventArgs reset_event_args = new(NotifyCollectionChangedAction.Reset);
}
