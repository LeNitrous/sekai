// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Rendering;

namespace Sekai.Processors;

public sealed class Camera2DProcessor : Processor<Camera2D>
{
    private readonly GraphicsContext context = Services.Current.Resolve<GraphicsContext>();

    protected override void Process(double delta, Camera2D camera)
    {
        var target = camera.Target ?? context.BackBufferTarget;
        var rotation = Matrix4x4.CreateRotationZ(camera.Owner?.Transform.Rotation ?? 0);
        var position = new Vector3(camera.Owner?.Transform.Position ?? Vector2.Zero, 0);

        camera.ViewMatrix = Matrix4x4.CreateLookAt(position, position + Vector3.Transform(-Vector3.UnitZ, rotation), Vector3.UnitY);
        camera.ProjMatrix = Matrix4x4.CreateOrthographicOffCenter(0, target.Width * camera.OrthoSize.X, target.Height * camera.OrthoSize.Y, 0, -1, 1);

        Matrix4x4.Invert(camera.ViewMatrix, out var viewMatrixInverse);
        camera.ViewMatrixInverse = viewMatrixInverse;

        Matrix4x4.Invert(camera.ProjMatrix, out var projMatrixInverse);
        camera.ProjMatrixInverse = projMatrixInverse;
    }
}
