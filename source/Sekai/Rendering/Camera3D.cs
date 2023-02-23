// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A camera capable of rendering three-dimensional objects.
/// </summary>
[Processor<Camera3DProcessor>]
public partial class Camera3D : Camera, IRenderObject
{
    /// <summary>
    /// The camera's near plane distance.
    /// </summary>
    public float NearPlane = 0.1f;

    /// <summary>
    /// The camera's far plane distance.
    /// </summary>
    public float FarPlane = 1000f;

    /// <summary>
    /// The camera's aspect ratio.
    /// </summary>
    public float AspectRatio = 16f / 9;

    /// <summary>
    /// The camera's field of view in degrees.
    /// </summary>
    public float FieldOfView = 60;

    /// <summary>
    /// The camera's mode of projection.
    /// </summary>
    public CameraProjectionMode Projection = CameraProjectionMode.Perspective;

    [Bind]
    internal Transform3D Transform { get; private set; } = null!;

    internal sealed override Transform GetTransform() => Transform;

    RenderKind IRenderObject.Kind => RenderKind.Render3D;
}

/// <summary>
/// Determines the projection mode of a <see cref="Camera3D"/>.
/// </summary>
public enum CameraProjectionMode
{
    /// <summary>
    /// Perspective projection.
    /// </summary>
    Perspective,

    /// <summary>
    /// Orthographic projection.
    /// </summary>
    Orthographic,
}
