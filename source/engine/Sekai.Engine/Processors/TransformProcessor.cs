// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Engine.Processors;

public sealed class TransformProcessor : Processor<Transform>
{
    protected override void OnEntityAdded(Entity entity, Transform component)
    {
    }

    protected override void OnEntityRemoved(Entity entity, Transform component)
    {
    }

    protected override void Update(Entity entity, Transform component)
    {
        component.LocalMatrix = Matrix4x4.CreateScale(component.Scale) * Matrix4x4.CreateFromQuaternion(component.Rotation) * Matrix4x4.CreateTranslation(component.Position);

        var parent = entity.Parent?.GetCommponent<Transform>();

        component.WorldMatrix = parent != null ? Matrix4x4.Multiply(component.LocalMatrix, parent.WorldMatrix) : component.LocalMatrix;
    }
}
