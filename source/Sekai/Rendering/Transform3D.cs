// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Processors;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A three-dimensional transform containing information about a <see cref="Node"/>'s position in a <see cref="Scene"/>.
/// </summary>
[Processor<Transform3DProcessor>]
public class Transform3D : Transform
{
    /// <summary>
    /// The transform's position.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The transform's scale.
    /// </summary>
    public Vector3 Scale = Vector3.One;

    /// <summary>
    /// The transform's rotation.
    /// </summary>
    public Vector3 Rotation;
}
