// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Rendering;

namespace Sekai.Processors;

internal sealed class Transform3DProcessor : TransformProcessor<Transform3D>
{
    protected override Matrix4x4 GetPositionMatrix(Transform3D transform)
        => Matrix4x4.CreateTranslation(transform.Position);

    protected override Matrix4x4 GetRotationMatrix(Transform3D transform)
        => Matrix4x4.CreateFromYawPitchRoll(transform.Rotation.Y, transform.Rotation.X, transform.Rotation.Z);

    protected override Matrix4x4 GetScaleMatrix(Transform3D transform)
        => Matrix4x4.CreateScale(transform.Scale);
}
