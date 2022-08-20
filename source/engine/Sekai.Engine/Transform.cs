// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Engine;
public class Transform : Component
{
    public Vector3 Position { get; set; }

    public Quaternion Rotation { get; set; }

    public Vector3 Scale { get; set; }
}
