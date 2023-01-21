// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Graphics.Textures;
using Sekai.Rendering;

namespace Sekai.Processors;

internal sealed class Camera3DProcessor : CameraProcessor<Camera3D>
{
    protected override void Update(Camera3D camera)
    {
        base.Update(camera);
        Renderer.Collect(camera);
    }

    protected override Vector3 GetPosition(Camera3D camera) => camera.Transform.Position;

    protected override Matrix4x4 GetRotation(Camera3D camera) => camera.Transform.RotationMatrix;

    protected override Matrix4x4 GetProjMatrix(IRenderTarget target, Camera3D camera) => camera.Projection switch
    {
        CameraProjectionMode.Perspective => Matrix4x4.CreatePerspectiveFieldOfView(camera.FieldOfView, camera.AspectRatio, camera.NearPlane, camera.FarPlane),
        CameraProjectionMode.Orthographic => Matrix4x4.CreateOrthographic(target.Width, target.Height, camera.NearPlane, camera.FarPlane),
        _ => throw new NotSupportedException(),
    };
}
