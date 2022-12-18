// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A camera capable of rendering three-dimensional objects.
/// </summary>
[Processor<Camera3DProcessor>]
public class Camera3D : Camera
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
    /// The camera projection mode.
    /// </summary>
    public CameraProjectionMode Projection;

    /// <inheritdoc cref="Component.Owner"/>
    public new Node3D? Owner => (Node3D?)base.Owner;

    /// <inheritdoc cref="Component.Scene"/>
    public new Scene3D? Scene => (Scene3D?)base.Scene;

    protected override void OnActivate()
    {
        base.OnActivate();
        Scene?.Renderer.Add(this);
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        Scene?.Renderer.Remove(this);
    }

    internal override bool CanAttach(Node node) => node is Node3D;
}

public enum CameraProjectionMode
{
    Perspective,
    Orthographic,
}
