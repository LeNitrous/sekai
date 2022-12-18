// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A node for <see cref="Scene2D"/>.
/// </summary>
[Processor<Transform2DProcessor>]
public sealed class Node2D : Node, IRenderableNode
{
    public Transform2D Transform = new();

    internal override bool CanAttach(Scene scene) => base.CanAttach(scene) || scene is Scene2D;
    internal override bool CanAttach(Node parent) => base.CanAttach(parent) || parent is Node2D;

    ITransform IRenderableNode.Transform => Transform;
}
