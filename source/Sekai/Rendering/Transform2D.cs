// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A two-dimensional transform containing information about a <see cref="Node"/>'s position in a <see cref="Scene"/>.
/// </summary>
[Processor<Transform2DProcessor>]
public class Transform2D : Transform
{
    /// <summary>
    /// The transform's position.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The transform's scale.
    /// </summary>
    public Vector2 Scale = Vector2.One;

    /// <summary>
    /// The transform's rotation in radians.
    /// </summary>
    public float Rotation;

    /// <summary>
    /// The transform's depth.
    /// </summary>
    public float Depth;
}
