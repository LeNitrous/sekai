// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Framework.Extensions;
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
        get => rotation;
        set
        {
            if (rotation == value)
                return;

            rotation = value;
            rotationEuler = value.ToEulerAngles();
        }
    }

    /// <summary>
    /// The rotation of the entity as euler angles in radians.
    /// </summary>
    public Vector3 RotationEuler
    {
        get => rotationEuler;
        set
        {
            if (rotationEuler == value)
                return;

            rotation = Quaternion.CreateFromYawPitchRoll(value.X, value.Y, value.Z);
            rotationEuler = value;
        }
    }

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
    public Matrix4x4 LocalMatrix = Matrix4x4.Identity;

    /// <summary>
    /// The world matrix of this transform.
    /// </summary>
    public Matrix4x4 WorldMatrix = Matrix4x4.Identity;

    private Quaternion rotation;
    private Vector3 rotationEuler;
}
