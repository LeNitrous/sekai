// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Processors;

public sealed class Transform2DProcessor : Processor<Node2D>
{
    protected override void Process(double delta, Node2D node)
    {
        var rot = Matrix4x4.CreateRotationZ(node.Transform.Rotation);
        var scale = Matrix4x4.CreateScale(new Vector3(node.Transform.Scale, 1));
        var trans = Matrix4x4.CreateTranslation(new Vector3(node.Transform.Position, 0));
        node.Transform.LocalMatrix = rot * scale * trans;
        node.Transform.WorldMatrix = node.Parent is Node2D parent ? parent.Transform.WorldMatrix * node.Transform.LocalMatrix : node.Transform.LocalMatrix;
    }
}
