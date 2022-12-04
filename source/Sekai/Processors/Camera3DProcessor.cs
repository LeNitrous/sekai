// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Mathematics;
using Sekai.Rendering;

namespace Sekai.Processors;

public sealed class Camera3DProcessor : Processor<Camera3D>
{
    private readonly GraphicsContext context = Services.Current.Resolve<GraphicsContext>();

    protected override void Process(double delta, Camera3D camera)
    {
        var target = camera.Target ?? context.BackBufferTarget;
        var position = camera.Owner?.Transform.Position ?? Vector3.Zero;
        var rotation = camera.Owner?.Transform.Rotation ?? Quaternion.Identity;

        camera.ViewMatrix = Matrix4x4.CreateLookAt(position, position + Vector3.Transform(-position, rotation), Vector3.UnitY);
        camera.ProjMatrix = camera.Projection == CameraProjectionMode.Perspective
            ? Matrix4x4.CreatePerspectiveFieldOfView(MathUtil.DegreesToRadians(camera.FieldOfView), camera.AspectRatio, camera.NearPlane, camera.FarPlane)
            : Matrix4x4.CreateOrthographic(target.Width, target.Height, camera.NearPlane, camera.FarPlane);

        var viewProjMatrix = camera.ProjMatrix * camera.ViewMatrix;
        camera.Frustum = new BoundingFrustum(ref viewProjMatrix);

        Matrix4x4.Invert(camera.ViewMatrix, out var viewMatrixInverse);
        camera.ViewMatrixInverse = viewMatrixInverse;

        Matrix4x4.Invert(camera.ProjMatrix, out var projMatrixInverse);
        camera.ProjMatrixInverse = projMatrixInverse;
    }
}
