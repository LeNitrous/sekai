// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Processors;

public sealed class Transform2DProcessor : TransformProcessor<Node2D>
{
    internal override void Update(Node2D node)
    {
        var rot = Matrix3x2.CreateRotation(node.Transform.Rotation);
        var scale = Matrix3x2.CreateScale(node.Transform.Scale);
        var trans = Matrix3x2.CreateTranslation(node.Transform.Position);
        node.Transform.LocalMatrix = new Matrix4x4(rot * scale * trans) * Matrix4x4.Identity;
        node.Transform.WorldMatrix = node.Parent is Node2D parent ? parent.Transform.WorldMatrix * node.Transform.LocalMatrix : node.Transform.LocalMatrix;
    }
}
