// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Processors;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// Base class for all attachable objects to extend a <see cref="Node"/>'s functionality.
/// </summary>
public abstract class Component : ActivateableObject, IReferenceable, IProcessorAttachable
{
    public Guid Id { get; }
    public Scene? Scene { get; private set; }

    /// <summary>
    /// The node owning this component.
    /// </summary>
    public Node? Owner { get; private set; }

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
    protected override void OnDeactivate() => Scene?.Processors.Detach(this);
    protected override void OnActivate() => Scene?.Processors.Attach(this);
    protected override void Destroy() => Owner?.Remove(this);
}
