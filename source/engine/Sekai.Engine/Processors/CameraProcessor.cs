// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Framework.Utils;
using Sekai.Framework.Windowing;

namespace Sekai.Engine.Processors;

public sealed class CameraProcessor : Processor<Camera, Transform>
{
    private readonly IView view = Game.Current.Services.Resolve<IView>();

    public CameraProcessor()
    {
        OnEntityAdded += handleEntityAdded;
        OnEntityRemoved += handleEntityRemoved;
    }

    private void handleEntityAdded(Processor processor, Entity entity)
    {
        Scene.RenderContext.Add(entity.GetCommponent<Camera>()!);
    }

    private void handleEntityRemoved(Processor processor, Entity entity)
    {
        Scene.RenderContext.Remove(entity.GetCommponent<Camera>()!);
    }

    protected override void Update(double delta, Entity entity, Camera camera, Transform transform)
    {
        var target = Vector3.Transform(-Vector3.UnitZ, transform.Rotation);
        camera.ViewMatrix = Matrix4x4.CreateLookAt(transform.Position, transform.Position + target, Vector3.UnitY);

        float fov = MathUtils.DegreesToRadians(camera.FieldOfView);
        float aspectRatio = (float)view.Size.Width / view.Size.Height;

        camera.ProjMatrix = camera.Projection == CameraProjectionMode.Perspective
            ? Matrix4x4.CreatePerspectiveFieldOfView(fov, aspectRatio, camera.NearClipPlane, camera.FarClipPlane)
            : Matrix4x4.CreateOrthographic(view.Size.Width, view.Size.Height, camera.NearClipPlane, camera.FarClipPlane);
    }

    protected override void Destroy()
    {
        OnEntityAdded -= handleEntityAdded;
        OnEntityRemoved -= handleEntityRemoved;
        base.Destroy();
    }
}
