// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Framework;

public sealed class Transform : Behavior
{
    public Vector3 Position;

    public Vector3 Scale;

    public Quaternion Rotation;

    public Matrix4x4 LocalMatrix { get; private set; } = Matrix4x4.Identity;

    public Matrix4x4 WorldMatrix { get; private set; } = Matrix4x4.Identity;
}
