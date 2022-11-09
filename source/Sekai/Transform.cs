// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Mathematics;

namespace Sekai;

public sealed class Transform : Behavior
{
    public Vector3 Position;

    public Vector3 Scale;

    public Quaternion Rotation;

    public Matrix LocalMatrix { get; private set; } = Matrix.Identity;

    public Matrix WorldMatrix { get; private set; } = Matrix.Identity;
}
