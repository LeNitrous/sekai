// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Engine;

/// <summary>
/// Represents a Transform for a Entity.
/// </summary>
public class Transform : Component
{
    /// <summary>
    /// Position of the Entity.
    /// </summary>
    public Vector3 Position { get; set; }
    /// <summary>
    /// Rotation of the Entity.
    /// </summary>
    public Quaternion Rotation { get; set; }
    /// <summary>
    /// Scale of the Entity.
    /// </summary>
    public Vector3 Scale { get; set; }
}
