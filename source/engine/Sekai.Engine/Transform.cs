// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Framework.Utils;

namespace Sekai.Engine;

/// <summary>
/// Represents a Transform for a Entity.
/// </summary>
public class Transform : Component
{
    /// <summary>
    /// The position of the entity.
    /// </summary>
    public Vector3 Position { get; set; }

    /// <summary>
    /// The rotation of the entity.
    /// </summary>
    public Quaternion Rotation
    {
        get => Quaternion.CreateFromYawPitchRoll(RotationEuler.X, RotationEuler.Y, RotationEuler.Z);
        set => RotationEuler = MathUtils.CreateEulerAnglesFromQuaternion(value);
    }

    /// <summary>
    /// The rotation of the entity as euler angles in radians.
    /// </summary>
    public Vector3 RotationEuler { get; set; }

    /// <summary>
    /// The scale of the entity.
    /// </summary>
    public Vector3 Scale { get; set; } = Vector3.One;

    /// <summary>
    /// The normalized forward vector of this transform.
    /// </summary>
    public Vector3 Forward => MathUtils.GetForward(Rotation);

    /// <summary>
    /// The normalized right vector of this transform.
    /// </summary>
    public Vector3 Right => MathUtils.GetRight(Rotation);

    /// <summary>
    /// The normalized up vector of this transform.
    /// </summary>
    public Vector3 Up => MathUtils.GetUp(Rotation);

    /// <summary>
    /// The local matrix of this transform.
    /// </summary>
    internal Matrix4x4 LocalMatrix = Matrix4x4.Identity;

    /// <summary>
    /// The world matrix of this transform.
    /// </summary>
    internal Matrix4x4 WorldMatrix = Matrix4x4.Identity;
}
