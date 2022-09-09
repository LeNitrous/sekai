// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Engine.Processors;

public class TransformProcessor : Processor<Transform>
{
    protected override void Load()
    {}
    protected override void Update(double elapsed, Entity entity, Transform component)
    {
        component.LocalMatrix = Matrix4x4.CreateTranslation(component.Position) * Matrix4x4.CreateScale(component.Scale * 0.1f) * Matrix4x4.CreateFromQuaternion(component.Rotation);
        component.WorldMatrix = entity.Parent?.HasComponent<Transform>() ?? false
            ? Matrix4x4.Multiply(component.LocalMatrix, entity.Parent.GetComponent<Transform>()!.WorldMatrix)
            : component.LocalMatrix;
    }
}
