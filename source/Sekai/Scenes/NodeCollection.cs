// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sekai.Scenes;

/// <summary>
/// Represents a collection of <see cref="Node"/>s.
/// </summary>
public abstract class NodeCollection : DependencyObject, ICollection<Node>, IReadOnlyList<Node>
{
    /// <summary>
    /// The number of nodes in this collection.
    /// </summary>
    public int Count => nodes.Count;

    /// <summary>
    /// Gets the node at a specified index.
    /// </summary>
    /// <returns>The node at a specified index.</returns>
    public Node this[int index] => nodes[index];

    /// <summary>
    /// Invoked when a node has been added to this collection.
    /// </summary>
    public event EventHandler<NodeEventArgs>? OnNodeAdded;

    /// <summary>
    /// Invoked when a node has been removed from this collection.
    /// </summary>
    public event EventHandler<NodeEventArgs>? OnNodeRemoved;

    /// <summary>
    /// The nodes in this collection.
    /// </summary>
    public IEnumerable<Node> Nodes
    {
        get => nodes;
        set
        {
            Clear();
            AddRange(value);
        }
    }

    private readonly List<Node> nodes = new();

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
    /// Gets the enumerator that iterates through this collection.
    /// </summary>
    public IEnumerator<Node> GetEnumerator() => nodes.GetEnumerator();

    protected override void Destroy() => Clear();

    bool ICollection<Node>.IsReadOnly => false;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
