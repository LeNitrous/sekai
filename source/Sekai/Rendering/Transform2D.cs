// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Rendering;

public struct Transform2D : ITransform
{
    public Vector2 Position { get; set; }
    public Vector2 Scale { get; set; }
    public float Rotation { get; set; }
    public Matrix4x4 WorldMatrix { get; internal set; }
    public Matrix4x4 LocalMatrix { get; internal set; }

    public Transform2D()
    {
        WorldMatrix = Matrix4x4.Identity;
        LocalMatrix = Matrix4x4.Identity;
    }
}
