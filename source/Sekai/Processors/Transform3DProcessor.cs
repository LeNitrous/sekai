// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Processors;

public sealed class Transform3DProcessor : Processor<Node3D>
{
    protected override void Process(double delta, Node3D node)
    {
        var rot = Matrix4x4.CreateFromQuaternion(node.Transform.Rotation);
        var scale = Matrix4x4.CreateScale(node.Transform.Scale);
        var trans = Matrix4x4.CreateTranslation(node.Transform.Position);

        node.Transform.LocalMatrix = rot * scale * trans;
        node.Transform.WorldMatrix = node.Parent is Node3D parent ? parent.Transform.WorldMatrix * node.Transform.LocalMatrix : node.Transform.LocalMatrix;

        Matrix4x4.Invert(node.Transform.WorldMatrix, out var inverse);
        node.Transform.WorldMatrixInverse = inverse;
    }
}
