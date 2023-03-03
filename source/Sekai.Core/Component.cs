// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Serialization;

namespace Sekai;

/// <summary>
/// A structure that is able to extend the capabilities of a <see cref="Node"/>.
/// </summary>
[Serializable]
public abstract class Component : IReferenceable
{
    public Guid Id { get; }

    /// <summary>
    /// The node owning this component.
    /// </summary>
    public Node? Owner { get; private set; }

    /// <summary>
    /// Gets whether this component is already attached or not.
    /// </summary>
    public bool IsAttached => Owner is not null;

    /// <summary>
    /// Creates a new instance of a <see cref="Component"/>.
    /// </summary>
    protected Component()
        : this(Guid.NewGuid())
    {
    }

    private Component(Guid id)
    {
        Id = id;
    }

    /// <summary>
    /// Attaches this component to <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The node attaching this component.</param>
    /// <exception cref="InvalidOperationException">Thrown when this component is already attached.</exception>
    internal void Attach(Node node)
    {
        if (IsAttached)
        {
            throw new InvalidOperationException("Cannot attach component to another node.");
        }

        Owner = node;
    }

    /// <summary>
    /// Detaches this component <paramref name="node"/>.
    /// </summary>
    /// <param name="node">The node detaching this component.</param>
    /// <exception cref="InvalidOperationException">Thrown when attempting to detach the component at an invalid state.</exception>
    internal void Detach(Node node)
    {
        if (Owner is null)
        {
            throw new InvalidOperationException("Cannot detach an ownerless component.");
        }

        if (!Owner.Id.Equals(node.Id))
        {
            throw new InvalidOperationException("Cannot detach from node not owning this component.");
        }

        Owner = null;
    }
}
