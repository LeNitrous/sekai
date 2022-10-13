// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Engine.Processors;

public sealed class TransformProcessor : Processor<Transform>
{
    protected override void Update(double delta, Entity entity, Transform component)
    {
        component.LocalMatrix = Matrix4x4.CreateTranslation(component.Position) * Matrix4x4.CreateScale(component.Scale * 0.1f) * Matrix4x4.CreateFromQuaternion(component.Rotation);

        var parent = entity.Parent?.GetCommponent<Transform>();

        component.WorldMatrix = parent != null
            ? Matrix4x4.Multiply(component.LocalMatrix, parent.WorldMatrix)
            : component.LocalMatrix;
    }
}
