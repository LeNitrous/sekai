// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Rendering;
using Sekai.Scenes;

namespace Sekai.Processors;

internal abstract class TransformProcessor<T> : Processor<T>
    where T : Transform
{
    public sealed override int Priority => int.MinValue;

    protected sealed override void Update(SceneCollection scenes, T transform)
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

internal sealed class Transform2DProcessor : TransformProcessor<Transform2D>
{
    protected override Matrix4x4 GetPositionMatrix(Transform2D transform)
        => Matrix4x4.CreateTranslation(new Vector3(transform.Position, 0));

    protected override Matrix4x4 GetRotationMatrix(Transform2D transform)
        => Matrix4x4.CreateRotationZ(transform.Rotation);

    protected override Matrix4x4 GetScaleMatrix(Transform2D transform)
        => Matrix4x4.CreateScale(new Vector3(transform.Scale, 1));
}

internal sealed class Transform3DProcessor : TransformProcessor<Transform3D>
{
    protected override Matrix4x4 GetPositionMatrix(Transform3D transform)
        => Matrix4x4.CreateTranslation(transform.Position);

    protected override Matrix4x4 GetRotationMatrix(Transform3D transform)
        => Matrix4x4.CreateFromYawPitchRoll(transform.Rotation.Y, transform.Rotation.X, transform.Rotation.Z);

    protected override Matrix4x4 GetScaleMatrix(Transform3D transform)
        => Matrix4x4.CreateScale(transform.Scale);
}
