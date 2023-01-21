// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Rendering;

namespace Sekai.Processors;

internal abstract class TransformProcessor<T> : Processor<T>
    where T : Transform
{
    public sealed override int Priority => int.MinValue;

    protected sealed override void Update(T transform)
    {
        T? parent = null;
        transform.Node?.Parent?.TryGetComponent(out parent);
        transform.PositionMatrix = GetPositionMatrix(transform);
        transform.RotationMatrix = GetRotationMatrix(transform);
        transform.ScaleMatrix = GetScaleMatrix(transform);
        transform.WorldMatrix = parent is not null ? parent.WorldMatrix * transform.LocalMatrix : transform.LocalMatrix;
    }

    protected abstract Matrix4x4 GetPositionMatrix(T transform);
    protected abstract Matrix4x4 GetRotationMatrix(T transform);
    protected abstract Matrix4x4 GetScaleMatrix(T transform);
}
