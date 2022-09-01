// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Engine.Rendering;

namespace Sekai.Engine.Processors;

public class TransformProcessor : Processor<Transform>
{
    private RenderContext renderContext = null!;

    protected override void Load()
    {
        renderContext = Systems.Get<RenderContext>();
    }
    protected override void Update(double elapsed, Entity entity, Transform component)
    {
        if (entity.Parent != null)
        {
            var parent = entity.Parent?.GetComponent<Transform>();
            component.LocalMatrix = Matrix4x4.CreateTranslation(component.Position) * Matrix4x4.CreateScale(component.Scale) * Matrix4x4.CreateFromQuaternion(component.Rotation);
            component.WorldMatrix = Matrix4x4.Multiply(component.LocalMatrix, parent.WorldMatrix);
        }
        else
        {
            component.LocalMatrix = Matrix4x4.CreateTranslation(component.Position) * Matrix4x4.CreateScale(component.Scale) * Matrix4x4.CreateFromQuaternion(component.Rotation);
            component.WorldMatrix = component.LocalMatrix;
        }
    }
}
