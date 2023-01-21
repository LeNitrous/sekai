// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Graphics.Textures;
using Sekai.Rendering;

namespace Sekai.Processors;

internal sealed class Camera2DProcessor : CameraProcessor<Camera2D>
{
    protected override void Update(Camera2D camera)
    {
        base.Update(camera);
        Renderer.Collect(camera);
    }

    protected override Vector3 GetPosition(Camera2D camera) => new(camera.Transform.Position, 0);

    protected override Matrix4x4 GetRotation(Camera2D camera) => camera.Transform.RotationMatrix;

    protected override Matrix4x4 GetProjMatrix(IRenderTarget target, Camera2D camera) => Matrix4x4.CreateOrthographicOffCenter(0, target.Width * camera.OrthoSize.X, target.Height * camera.OrthoSize.Y, 0, -1, 1);
}
