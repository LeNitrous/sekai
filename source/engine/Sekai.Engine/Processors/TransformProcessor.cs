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
        var trans = Matrix4x4.CreateTranslation(component.Position);
        var scale = Matrix4x4.CreateScale(component.Scale);
        component.LocalMatrix =  Matrix4x4.Transform(trans * scale, component.Rotation);

        var world = entity.Parent?.GetComponent<Transform>()?.WorldMatrix ?? renderContext.WorldMatrix;
        component.WorldMatrix = Matrix4x4.Multiply(component.LocalMatrix, world);
    }
}
