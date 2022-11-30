// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A node for <see cref="Scene2D"/>.
/// </summary>
public sealed class Node2D : Node, IRenderableNode
{
    public Transform2D Transform;

    protected override void OnAttach()
    {
        base.OnAttach();
        Scene?.Get<Transform2DProcessor>().Add(this);
    }

    protected override void OnDetach()
    {
        base.OnDetach();
        Scene?.Get<Transform2DProcessor>().Remove(this);
    }

    internal override bool CanAttach(Scene scene) => base.CanAttach(scene) || scene is Scene2D;
    internal override bool CanAttach(Node parent) => base.CanAttach(parent) || parent is Node2D;

    ITransform IRenderableNode.Transform => Transform;
}
