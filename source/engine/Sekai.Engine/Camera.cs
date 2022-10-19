// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;

namespace Sekai.Engine;

/// <summary>
/// Represent a camera in the scene.
/// </summary>
public class Camera : Component
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
    /// Gets or sets the camera's view width.
    /// </summary>
    public float Width
    {
        get => Size.Width;
        set => Size = new SizeF(value, Size.Height);
    }

    /// <summary>
    /// Gets or sets the camera's view height.
    /// </summary>
    public float Height
    {
        get => Size.Height;
        set => Size = new SizeF(Size.Width, value);
    }

    /// <summary>
    /// Gets or sets the camera's view size.
    /// </summary>
    public SizeF Size = new(512f, 512f);
}

/// <summary>
/// Camera projection mode
/// </summary>
public enum CameraProjectionMode
{
    Perspective,
    Orthographic,
}
