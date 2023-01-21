// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Textures;
using Sekai.Rendering;

namespace Sekai.Processors;

internal abstract class CameraProcessor<T> : Processor<T>
    where T : Camera
{
    [Resolved]
    private GraphicsContext graphics { get; set; } = null!;

    [Resolved]
    protected Renderer Renderer { get; private set; } = null!;

    protected override void Update(T camera)
    {
        var target = camera.Target ?? graphics.BackBufferTarget;
        var position = GetPosition(camera);
        var rotation = GetRotation(camera);
        camera.ViewMatrix = Matrix4x4.CreateLookAt(position, position + Vector3.Transform(-Vector3.UnitZ, rotation), Vector3.UnitY);
        camera.ProjMatrix = GetProjMatrix(target, camera);
    }

    protected abstract Vector3 GetPosition(T camera);
    protected abstract Matrix4x4 GetRotation(T camera);
    protected abstract Matrix4x4 GetProjMatrix(IRenderTarget target, T camera);
}
