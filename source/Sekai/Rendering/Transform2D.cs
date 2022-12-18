// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Rendering;

/// <summary>
/// A two-dimensional transform.
/// </summary>
public struct Transform2D : ITransform, IEquatable<Transform2D>, IComparable<Transform2D>
{
    /// <summary>
    /// The transform's position.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The transform's scale.
    /// </summary>
    public Vector2 Scale;

    /// <summary>
    /// The transform's rotation in radians.
    /// </summary>
    public float Rotation;

    /// <summary>
    /// The transform's depth.
    /// </summary>
    public float Depth;

    public Matrix4x4 WorldMatrix { get; internal set; }
    public Matrix4x4 LocalMatrix { get; internal set; }
    public Matrix4x4 WorldMatrixInverse { get; internal set; }

    public Transform2D()
    {
        Scale = Vector2.One;
        LocalMatrix = Matrix4x4.Identity;
        WorldMatrix = Matrix4x4.Identity;
        WorldMatrixInverse = Matrix4x4.Identity;
    }

    public int CompareTo(Transform2D other) => Depth.CompareTo(other.Depth);

    public bool Equals(Transform2D other)
    {
        return Position.Equals(other.Position)
            && Scale.Equals(other.Scale)
            && Rotation.Equals(other.Rotation)
            && Depth.Equals(other.Depth)
            && WorldMatrix.Equals(other.WorldMatrix)
            && LocalMatrix.Equals(other.LocalMatrix);
    }

    public override bool Equals(object? obj) => obj is Transform2D d && Equals(d);

    public override int GetHashCode() => HashCode.Combine(Position, Scale, Rotation, Depth, WorldMatrix, LocalMatrix);

    public static bool operator ==(Transform2D left, Transform2D right) => left.Equals(right);

    public static bool operator !=(Transform2D left, Transform2D right) => !(left == right);

    public static bool operator <(Transform2D left, Transform2D right) => left.CompareTo(right) < 0;

    public static bool operator <=(Transform2D left, Transform2D right) => left.CompareTo(right) <= 0;

    public static bool operator >(Transform2D left, Transform2D right) => left.CompareTo(right) > 0;

    public static bool operator >=(Transform2D left, Transform2D right) => left.CompareTo(right) >= 0;
}
