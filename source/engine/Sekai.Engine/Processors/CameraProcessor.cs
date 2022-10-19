// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Numerics;
using Sekai.Engine.Rendering;
using Sekai.Framework.Utils;

namespace Sekai.Engine.Processors;

public sealed class CameraProcessor : Processor<Camera, Transform>
{
    private readonly Dictionary<Camera, CameraInfo> cameras = new();

    protected override void OnEntityAdded(Entity entity, Camera camera, Transform transform)
    {
        if (cameras.ContainsKey(camera))
            return;

        var info = new CameraInfo();
        cameras.Add(camera, info);
        Scene.RenderContext.AddCamera(info);
    }

    protected override void OnEntityRemoved(Entity entity, Camera camera, Transform transform)
    {
        if (cameras.Remove(camera, out var info))
            Scene.RenderContext.RemoveCamera(info);
    }

    protected override void Update(double delta, Entity entity, Camera camera, Transform transform)
    {
        if (!cameras.TryGetValue(camera, out var info))
            return;

        var target = Vector3.Transform(-Vector3.UnitZ, transform.Rotation);
        info.ViewMatrix = Matrix4x4.CreateLookAt(transform.Position, transform.Position + target, Vector3.UnitY);

        info.ProjMatrix = camera.Projection == CameraProjectionMode.Perspective
            ? Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DegreesToRadians(camera.FieldOfView), (float)camera.Width / camera.Height, camera.NearClipPlane, camera.FarClipPlane)
            : Matrix4x4.CreateOrthographicOffCenter(0, camera.Width, camera.Height, 0, -1, 1);
    }
}
