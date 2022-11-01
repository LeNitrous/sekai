// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Mathematics;

public unsafe struct BoundingFrustum : IEquatable<BoundingFrustum>
{
    public static readonly BoundingFrustum Empty = new();

    public Plane Left;
    public Plane Right;
    public Plane Bottom;
    public Plane Top;
    public Plane Near;
    public Plane Far;

    public BoundingFrustum(Plane left, Plane right, Plane bottom, Plane top, Plane near, Plane far)
    {
        Left = left;
        Right = right;
        Bottom = bottom;
        Top = top;
        Near = near;
        Far = far;
    }

    public BoundingFrustum(Matrix4x4 matrix)
    {
        Left = Plane.Normalize
        (
            new Plane
            (
                matrix.M14 + matrix.M11,
                matrix.M24 + matrix.M21,
                matrix.M34 + matrix.M31,
                matrix.M44 + matrix.M41
            )
        );

        Right = Plane.Normalize
        (
            new Plane
            (
                matrix.M14 - matrix.M11,
                matrix.M24 - matrix.M21,
                matrix.M34 - matrix.M31,
                matrix.M44 - matrix.M41
            )
        );

        Bottom = Plane.Normalize
        (
            new Plane
            (
                matrix.M14 + matrix.M12,
                matrix.M24 + matrix.M22,
                matrix.M34 + matrix.M32,
                matrix.M44 + matrix.M42
            )
        );

        Top = Plane.Normalize
        (
            new Plane
            (
                matrix.M14 - matrix.M12,
                matrix.M24 - matrix.M22,
                matrix.M34 - matrix.M32,
                matrix.M44 - matrix.M42
            )
        );

        Near = Plane.Normalize
        (
            new Plane
            (
                matrix.M13,
                matrix.M23,
                matrix.M33,
                matrix.M43
            )
        );

        Far = Plane.Normalize
        (
            new Plane
            (
                matrix.M14 - matrix.M13,
                matrix.M24 - matrix.M23,
                matrix.M34 - matrix.M33,
                matrix.M44 - matrix.M43
            )
        );
    }

    public override bool Equals(object? obj)
    {
        return obj is BoundingFrustum frustrum && Equals(frustrum);

    }

    public bool Equals(BoundingFrustum other)
    {
        return Left.Equals(other.Left) &&
               Right.Equals(other.Right) &&
               Bottom.Equals(other.Bottom) &&
               Top.Equals(other.Top) &&
               Near.Equals(other.Near) &&
               Far.Equals(other.Far);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Left, Right, Bottom, Top, Near, Far);
    }

    public static bool operator ==(BoundingFrustum left, BoundingFrustum right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoundingFrustum left, BoundingFrustum right)
    {
        return !(left == right);
    }
}
