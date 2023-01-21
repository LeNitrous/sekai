// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// A component that contains information about a <see cref="Node"/>'s position in a <see cref="Scene"/>.
/// </summary>
public abstract class Transform : Component
{
    /// <summary>
    /// The transform's matrix in world space.
    /// </summary>
    public Matrix4x4 WorldMatrix { get; internal set; } = Matrix4x4.Identity;

    /// <summary>
    /// The transform's matrix in local space.
    /// </summary>
    public Matrix4x4 LocalMatrix => RotationMatrix * ScaleMatrix * PositionMatrix;

    /// <summary>
    /// The transform's position matrix.
    /// </summary>
    internal Matrix4x4 PositionMatrix { get; set; } = Matrix4x4.Identity;

    /// <summary>
    /// The transform's rotation matrix.
    /// </summary>
    internal Matrix4x4 RotationMatrix { get; set; } = Matrix4x4.Identity;

    /// <summary>
    /// The transform's scale matrix.
    /// </summary>
    internal Matrix4x4 ScaleMatrix { get; set; } = Matrix4x4.Identity;
}
