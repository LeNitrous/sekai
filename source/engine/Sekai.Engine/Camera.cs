// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Engine;
public class Camera : Component
{
    private struct CameraInset
    {
        internal float Left;
        internal float Right;
        internal float Top;
        internal float Bottom;
    }

    public float PositionZ3D = 2000f;

    public float NearClipPlane = 0.0001f;

    public float FarClipPlane = 5000f;
}
