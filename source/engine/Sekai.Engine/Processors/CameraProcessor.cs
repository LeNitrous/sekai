// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Framework.Annotations;
using Sekai.Framework.Utils;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Processors;

public class CameraProcessor : Processor<Camera, Transform>
{
    [Resolved]
    private IView window { get; set; } = null!;

    protected override void Update(double elapsed, Entity entity, Camera camera, Transform transform)
    {
        var target = Vector3.Transform(-Vector3.UnitZ, transform.Rotation);
        camera.ViewMatrix = Matrix4x4.CreateLookAt(transform.Position, transform.Position + target, Vector3.UnitY);

        float fov = MathUtils.DegreesToRadians(camera.FieldOfView);
        float aspectRatio = (float)window.Size.Width / window.Size.Height;

        camera.ProjMatrix = camera.Projection == CameraProjectionMode.Perspective
            ? Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, camera.NearClipPlane, camera.FarClipPlane)
            : Matrix4x4.CreateOrthographic(window.Size.Width, window.Size.Height, camera.NearClipPlane, camera.FarClipPlane);
    }
}
