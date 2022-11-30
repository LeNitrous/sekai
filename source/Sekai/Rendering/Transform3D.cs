// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Rendering;

public struct Transform3D : ITransform
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Quaternion Rotation { get; set; }
    public Matrix4x4 WorldMatrix { get; internal set; }
    public Matrix4x4 LocalMatrix { get; internal set; }

    public Transform3D()
    {
        WorldMatrix = Matrix4x4.Identity;
        LocalMatrix = Matrix4x4.Identity;
    }
}
