// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Rendering;

namespace Sekai.Processors;

internal sealed class Transform2DProcessor : TransformProcessor<Transform2D>
{
    protected override Matrix4x4 GetPositionMatrix(Transform2D transform)
        => Matrix4x4.CreateTranslation(new Vector3(transform.Position, 0));

    protected override Matrix4x4 GetRotationMatrix(Transform2D transform)
        => Matrix4x4.CreateRotationZ(transform.Rotation);

    protected override Matrix4x4 GetScaleMatrix(Transform2D transform)
        => Matrix4x4.CreateScale(new Vector3(transform.Scale, 1));
}
