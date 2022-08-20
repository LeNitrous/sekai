// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine;

/// <summary>
/// Represent a camera in the scene.
/// </summary>
public class Camera : Component
{
    private struct CameraInset
    {
        internal float Left;
        internal float Right;
        internal float Top;
        internal float Bottom;
    }

    /// <summary>
    /// Z-position of the 3D Camera projections. Affects FOV greatly.
    /// </summary>
    public float PositionZ3D = 2000f;

    /// <summary>
    /// Near Clip Plane of the 3D Camera.
    /// </summary>
    public float NearClipPlane = 0.0001f;

    /// <summary>
    /// Far Clip Plane of the 3D camera projection.
    /// </summary>
    public float FarClipPlane = 5000f;
}
