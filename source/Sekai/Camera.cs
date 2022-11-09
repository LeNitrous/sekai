// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using Sekai.Mathematics;
using Sekai.Rendering;

namespace Sekai;

/// <summary>
/// Represent a camera in the scene.
/// </summary>
public class Camera : Behavior
{
    /// <summary>
    /// The field of view of the camera in degrees. Used when <see cref="CameraProjectionMode.Perspective"/> is used.
    /// </summary>
    public float FieldOfView = 90.0f;

    /// <summary>
    /// The near clip plane of the camera.
    /// </summary>
    public float NearClipPlane = 0.0001f;

    /// <summary>
    /// The far clip plane of the camera.
    /// </summary>
    public float FarClipPlane = 5000f;

    /// <summary>
    /// The projection mode of the camera.
    /// </summary>
    public CameraProjectionMode Projection = CameraProjectionMode.Perspective;

    /// <summary>
    /// The camera's view width.
    /// </summary>
    public int Width
    {
        get => Size.Width;
        set => Size = new Size(value, Size.Height);
    }

    /// <summary>
    /// The camera's view height.
    /// </summary>
    public int Height
    {
        get => Size.Height;
        set => Size = new Size(Size.Width, value);
    }

    /// <summary>
    /// The camera's view size.
    /// </summary>
    public Size Size = new(512, 512);

    /// <summary>
    /// The render groups visible to this camera.
    /// </summary>
    public RenderGroup Groups = RenderGroup.Default;

    /// <summary>
    /// The culling method to use for this camera.
    /// </summary>
    public CullingMode Culling = CullingMode.Frustum;

    /// <summary>
    /// The camera's view matrix.
    /// </summary>
    public Matrix ViewMatrix { get; private set; } = Matrix.Identity;

    /// <summary>
    /// The camera's projection matrix.
    /// </summary>
    public Matrix ProjMatrix { get; private set; } = Matrix.Identity;

    /// <summary>
    /// The render target to draw on.
    /// </summary>
    /// <remarks>
    /// Set to null to draw to the default render target.
    /// </remarks>
    // public RenderTarget? RenderTarget;

    protected override void OnAttach()
    {
        base.OnAttach();
        // Game.Resolve<GameSystemCollection>().Get<Renderer>().Add(this);
    }

    protected override void OnDetach()
    {
        base.OnDetach();
        // Game.Resolve<GameSystemCollection>().Get<Renderer>().Add(this);
    }

    public override void Update(double delta)
    {
        if (Entity.Transform is null)
            return;

        var transform = Entity.Transform;

        ViewMatrix = Matrix.LookAtRH(transform.Position, transform.Position + Vector3.Transform(-Vector3.UnitZ, transform.Rotation), Vector3.UnitY);
        ProjMatrix = Projection == CameraProjectionMode.Perspective
            ? Matrix.PerspectiveFovRH(MathUtil.DegreesToRadians(FieldOfView), Width / (float)Height, NearClipPlane, FarClipPlane)
            : Matrix.OrthoOffCenterRH(0, Width, Height, 0, NearClipPlane, FarClipPlane);
    }
}

/// <summary>
/// Camera projection mode
/// </summary>
public enum CameraProjectionMode
{
    Perspective,
    Orthographic,
}
