// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Rendering;

namespace Sekai.Scenes;

/// <summary>
/// A node for <see cref="Scene3D"/>.
/// </summary>
public sealed class Node3D : Node, IRenderableNode
{
    public Transform3D Transform;

    protected override void OnAttach()
    {
        base.OnAttach();
        Scene?.Get<Transform3DProcessor>().Add(this);
    }

    protected override void OnDetach()
    {
        base.OnDetach();
        Scene?.Get<Transform3DProcessor>().Remove(this);
    }

    internal override bool CanAttach(Scene scene) => base.CanAttach(scene) && scene is Scene3D;
    internal override bool CanAttach(Node parent) => base.CanAttach(parent) && parent is Node3D;

    ITransform IRenderableNode.Transform => Transform;
}
