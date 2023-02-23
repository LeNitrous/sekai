// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Textures;
using Sekai.Rendering;

namespace Sekai.Processors;

internal abstract partial class CameraProcessor<T> : Processor<T>
    where T : Camera
{
    [Resolved]
    private Renderer renderer { get; set; } = null!;

    [Resolved]
    private GraphicsContext graphics { get; set; } = null!;

    protected sealed override void Update(T camera)
    {
        var target = camera.Target ?? graphics.DefaultRenderTarget;
        var position = GetPosition(camera);
        var rotation = GetRotation(camera);
        camera.ViewMatrix = Matrix4x4.CreateLookAt(position, position + Vector3.Transform(-Vector3.UnitZ, rotation), Vector3.UnitY);
        camera.ProjMatrix = GetProjMatrix(target, camera);
        renderer.Collect((IRenderObject)camera);
    }

    protected abstract Vector3 GetPosition(T camera);
    protected abstract Matrix4x4 GetRotation(T camera);
    protected abstract Matrix4x4 GetProjMatrix(IRenderTarget target, T camera);
}

internal sealed class Camera2DProcessor : CameraProcessor<Camera2D>
{
    protected override Vector3 GetPosition(Camera2D camera) => new(camera.Transform.Position, 0);

    protected override Matrix4x4 GetRotation(Camera2D camera) => camera.Transform.RotationMatrix;

    protected override Matrix4x4 GetProjMatrix(IRenderTarget target, Camera2D camera) => Matrix4x4.CreateOrthographicOffCenter(0, target.Width * camera.OrthoSize.X, target.Height * camera.OrthoSize.Y, 0, -1, 1);
}

internal sealed class Camera3DProcessor : CameraProcessor<Camera3D>
{
    protected override Vector3 GetPosition(Camera3D camera) => camera.Transform.Position;

    protected override Matrix4x4 GetRotation(Camera3D camera) => camera.Transform.RotationMatrix;

    protected override Matrix4x4 GetProjMatrix(IRenderTarget target, Camera3D camera) => camera.Projection switch
    {
        CameraProjectionMode.Perspective => Matrix4x4.CreatePerspectiveFieldOfView(camera.FieldOfView, camera.AspectRatio, camera.NearPlane, camera.FarPlane),
        CameraProjectionMode.Orthographic => Matrix4x4.CreateOrthographic(target.Width, target.Height, camera.NearPlane, camera.FarPlane),
        _ => throw new NotSupportedException(),
    };
}
