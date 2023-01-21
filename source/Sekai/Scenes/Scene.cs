// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Allocation;
using Sekai.Rendering;
using Sekai.Serialization;

namespace Sekai.Scenes;

/// <summary>
/// A collection of nodes where it can process and render its children's components.
/// </summary>
[Serializable]
public class Scene : NodeCollection, IReferenceable
{
    public Guid Id { get; }

    [Resolved]
    private Renderer renderer { get; set; } = null!;

    private readonly ComponentManager manager = new();

    public Scene()
    {
        OnNodeAdded += handleNodeAdded;
        OnNodeRemoved += handleNodeRemoved;
    }

    public void Update()
    {
        renderer.Begin();
        manager.Update();
        renderer.End();
    }

    public void Render()
    {
        renderer.Render();
    }

    private void handleNodeAdded(object? sender, NodeEventArgs args)
    {
        args.Node.Scene = this;
        args.Node.OnNodeAdded += handleNodeAdded;
        args.Node.OnNodeRemoved += handleNodeRemoved;
        args.Node.OnComponentAdded += handleComponentAdded;
        args.Node.OnComponentRemoved += handleComponentRemoved;
        manager.Add(args.Node);
    }

    private void handleNodeRemoved(object? sender, NodeEventArgs args)
    {
        args.Node.Scene = null;
        args.Node.OnNodeAdded -= handleNodeAdded;
        args.Node.OnNodeRemoved -= handleNodeRemoved;
        args.Node.OnComponentAdded -= handleComponentAdded;
        args.Node.OnComponentRemoved -= handleComponentRemoved;
        manager.Remove(args.Node);
    }

    private void handleComponentAdded(object? sender, ComponentEventArgs args)
        => manager.Add(args.Component);

    private void handleComponentRemoved(object? sender, ComponentEventArgs args)
        => manager.Remove(args.Component);

    protected sealed override void Destroy()
    {
        base.Destroy();
        OnNodeAdded -= handleNodeAdded;
        OnNodeRemoved -= handleNodeRemoved;
    }
}
