// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Serialization;

namespace Sekai.Scenes;

public abstract class Component : ActivateableObject, IReferenceable
{
    public Guid Id { get; }

    /// <summary>
    /// The node owning this component.
    /// </summary>
    public Node? Owner { get; private set; }

    /// <summary>
    /// The scene where this component is attached to.
    /// </summary>
    public Scene? Scene { get; private set; }

    internal void Attach(Node node)
    {
        if (IsAttached)
            return;

        if (!CanAttach(node))
            throw new InvalidOperationException();

        Scene = node.Scene;
        Owner = node;

        Attach();
    }

    internal void Detach(Node node)
    {
        if (!IsAttached)
            return;

        if (Owner != node)
            throw new InvalidOperationException();

        Scene = null;
        Owner = null;

        Detach();
    }

    internal virtual bool CanAttach(Node node) => true;

    protected override void Destroy() => Owner?.Remove(this);
}
