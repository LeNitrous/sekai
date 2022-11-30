// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Processors;

public sealed class Transform3DProcessor : TransformProcessor<Node3D>
{
    internal override void Update(Node3D node)
    {
        var rot = Matrix4x4.CreateFromQuaternion(node.Transform.Rotation);
        var scale = Matrix4x4.CreateScale(node.Transform.Scale);
        var trans = Matrix4x4.CreateTranslation(node.Transform.Position);
        node.Transform.LocalMatrix = rot * scale * trans;
        node.Transform.WorldMatrix = node.Parent is Node3D parent ? parent.Transform.WorldMatrix * node.Transform.LocalMatrix : node.Transform.LocalMatrix;
    }
}
