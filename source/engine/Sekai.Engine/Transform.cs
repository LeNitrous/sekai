// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Engine.Extensions;

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
        set => RotationEuler = value.ToEulerAngles();
    }

    /// <summary>
    /// The rotation of the entity as euler angles.
    /// </summary>
    public Vector3 RotationEuler
    {
        get => rotationEuler;
        set => rotationEuler = new Vector3(((value.X % 360) + 360) % 360, ((value.Y % 360) + 360) % 360, ((value.Z % 360) + 360) % 360);
    }

    /// <summary>
    /// The scale of the entity.
    /// </summary>
    public Vector3 Scale { get; set; } = Vector3.One;

    /// <summary>
    /// The normalized forward vector of this transform.
    /// </summary>
    public Vector3 Forward => Rotation.GetForward();

    /// <summary>
    /// The normalized right vector of this transform.
    /// </summary>
    public Vector3 Right => Rotation.GetRight();

    /// <summary>
    /// The normalized up vector of this transform.
    /// </summary>
    public Vector3 Up => Rotation.GetUp();

    /// <summary>
    /// The local matrix of this transform.
    /// </summary>
    internal Matrix4x4 LocalMatrix = Matrix4x4.Identity;

    /// <summary>
    /// The world matrix of this transform.
    /// </summary>
    internal Matrix4x4 WorldMatrix = Matrix4x4.Identity;

    private Vector3 rotationEuler;
}
