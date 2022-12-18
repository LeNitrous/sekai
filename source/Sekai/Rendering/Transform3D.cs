// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Rendering;

/// <summary>
/// A three-dimensional transform.
/// </summary>
public struct Transform3D : ITransform, IEquatable<Transform3D>, IComparable<Transform3D>
{
    /// <summary>
    /// The transform's position.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The transform's scale.
    /// </summary>
    public Vector3 Scale;

    /// <summary>
    /// The transform's rotation.
    /// </summary>
    public Quaternion Rotation;

    public Matrix4x4 WorldMatrix { get; internal set; }
    public Matrix4x4 LocalMatrix { get; internal set; }
    public Matrix4x4 WorldMatrixInverse { get; internal set; }

    public Transform3D()
    {
        Scale = Vector3.One;
        Rotation = Quaternion.Identity;
        WorldMatrix = Matrix4x4.Identity;
        LocalMatrix = Matrix4x4.Identity;
        WorldMatrixInverse = Matrix4x4.Identity;
    }

    public int CompareTo(Transform3D other) => other.Position.Z.CompareTo(Position.Z);

    public bool Equals(Transform3D other)
    {
        return Position.Equals(other.Position)
            && Scale.Equals(other.Scale)
            && Rotation.Equals(other.Rotation)
            && WorldMatrix.Equals(other.WorldMatrix)
            && LocalMatrix.Equals(other.LocalMatrix);
    }

    public override bool Equals(object? obj) => obj is Transform3D d && Equals(d);

    public override int GetHashCode() => HashCode.Combine(Position, Scale, Rotation, WorldMatrix, LocalMatrix);

    public static bool operator ==(Transform3D left, Transform3D right) => left.Equals(right);

    public static bool operator !=(Transform3D left, Transform3D right) => !(left == right);

    public static bool operator <(Transform3D left, Transform3D right) => left.CompareTo(right) < 0;

    public static bool operator <=(Transform3D left, Transform3D right) => left.CompareTo(right) <= 0;

    public static bool operator >(Transform3D left, Transform3D right) => left.CompareTo(right) > 0;

    public static bool operator >=(Transform3D left, Transform3D right) => left.CompareTo(right) >= 0;
}
