// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Mathematics;

/// <summary>
/// Represents a bounding frustum in three-dimensional space.
/// </summary>
public struct BoundingFrustum : IContainable<BoundingFrustum, Vector3>, IContainable<BoundingFrustum, Ray>, IContainable<BoundingFrustum, Plane>, IContainable<BoundingFrustum, BoundingBox>, IContainable<BoundingFrustum, BoundingFrustum>, IContainable<BoundingFrustum, BoundingSphere>, IEquatable<BoundingFrustum>
{
    /// <summary>
    /// The left plane of this frustum.
    /// </summary>
    public Plane Left;

    /// <summary>
    /// The right plane of this frustum.
    /// </summary>
    public Plane Right;

    /// <summary>
    /// The top plane of this frustum.
    /// </summary>
    public Plane Top;

    /// <summary>
    /// The bottom plane of this frustum.
    /// </summary>
    public Plane Bottom;

    /// <summary>
    /// The near plane of this frustum.
    /// </summary>
    public Plane Near;

    /// <summary>
    /// The far plane of this frustum.
    /// </summary>
    public Plane Far;

    public BoundingFrustum(Plane left, Plane right, Plane top, Plane bottom, Plane near, Plane far)
    {
        Left = left;
        Right = right;
        Top = top;
        Bottom = bottom;
        Near = near;
        Far = far;
    }

    public Corners GetCorners()
    {
        Corners result = default;

        planeIntersect(ref Near, ref Top, ref Left, out result.NearTopLeft);
        planeIntersect(ref Near, ref Top, ref Right, out result.NearTopRight);
        planeIntersect(ref Near, ref Bottom, ref Left, out result.NearBottomLeft);
        planeIntersect(ref Near, ref Bottom, ref Right, out result.NearBottomRight);
        planeIntersect(ref Far, ref Top, ref Left, out result.FarTopLeft);
        planeIntersect(ref Far, ref Top, ref Right, out result.FarTopRight);
        planeIntersect(ref Far, ref Bottom, ref Left, out result.FarBottomLeft);
        planeIntersect(ref Far, ref Bottom, ref Right, out result.FarBottomRight);

        return result;
    }

    private static void planeIntersect(ref Plane p1, ref Plane p2, ref Plane p3, out Vector3 intersection)
    {
        intersection = (-(p1.D * Vector3.Cross(p2.Normal, p3.Normal))
                        - (p2.D * Vector3.Cross(p3.Normal, p1.Normal))
                        - (p3.D * Vector3.Cross(p1.Normal, p2.Normal)))
                        / Vector3.Dot(p1.Normal, Vector3.Cross(p2.Normal, p3.Normal));
    }

    public readonly bool Equals(BoundingFrustum other)
    {
        return Left.Equals(other.Left) &&
               Right.Equals(other.Right) &&
               Top.Equals(other.Top) &&
               Bottom.Equals(other.Bottom) &&
               Near.Equals(other.Near) &&
               Far.Equals(other.Far);
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is BoundingFrustum frustum && Equals(frustum);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Left, Right, Top, Bottom, Near, Far);
    }

    public static BoundingFrustum FromMatrix(Matrix4x4 matrix)
    {
        BoundingFrustum frustum = default;

        frustum.Left = Plane.Normalize(new
        (
            matrix.M14 + matrix.M11,
            matrix.M24 + matrix.M21,
            matrix.M34 + matrix.M31,
            matrix.M44 + matrix.M41
        ));

        frustum.Right = Plane.Normalize(new
        (
            matrix.M14 - matrix.M11,
            matrix.M24 - matrix.M21,
            matrix.M34 - matrix.M31,
            matrix.M44 - matrix.M41
        ));

        frustum.Top = Plane.Normalize(new
        (
            matrix.M14 - matrix.M12,
            matrix.M24 - matrix.M22,
            matrix.M34 - matrix.M32,
            matrix.M44 - matrix.M42
        ));

        frustum.Bottom = Plane.Normalize(new
        (
            matrix.M14 + matrix.M12,
            matrix.M24 + matrix.M22,
            matrix.M34 + matrix.M32,
            matrix.M44 + matrix.M42
        ));

        frustum.Near = Plane.Normalize(new
        (
            matrix.M13,
            matrix.M23,
            matrix.M33,
            matrix.M43
        ));

        frustum.Far = Plane.Normalize(new
        (
            matrix.M14 - matrix.M13,
            matrix.M24 - matrix.M23,
            matrix.M34 - matrix.M33,
            matrix.M44 - matrix.M43
        ));

        return frustum;
    }

    public static Containment Contains(BoundingFrustum frustum, Vector3 point)
    {
        var planes = MemoryMarshal.CreateReadOnlySpan(ref frustum.Left, 6);

        for (int i = 0; i < planes.Length; i++)
        {
            if (Plane.DotCoordinate(planes[i], point) < 0)
            {
                return Containment.Disjoint;
            }
        }

        return Containment.Contains;
    }

    public static Containment Contains(BoundingFrustum frustum, Ray ray)
    {
        var planes = MemoryMarshal.CreateReadOnlySpan(ref frustum.Left, 6);

        float tmin = float.MinValue;
        float tmax = float.MaxValue;

        for (int i = 0; i < planes.Length; i++)
        {
            float denominator = Vector3.Dot(planes[i].Normal, ray.Direction);

            if (MathF.Abs(denominator) < float.Epsilon)
            {
                if (Plane.DotCoordinate(planes[i], ray.Origin) < 0)
                {
                    return Containment.Disjoint;
                }

                continue;
            }

            float t = (-planes[i].D - Vector3.Dot(planes[i].Normal, ray.Origin)) / denominator;

            if (denominator > 0)
            {
                tmin = MathF.Max(tmin, t);
            }
            else
            {
                tmax = MathF.Min(tmax, t);
            }

            if (tmin > tmax)
            {
                return Containment.Disjoint;
            }
        }

        if (tmax >= tmin)
        {
            return Containment.Contains;
        }

        return Containment.Intersects;
    }

    public static Containment Contains(BoundingFrustum frustum, Plane plane)
    {
        var planes = MemoryMarshal.CreateReadOnlySpan(ref frustum.Left, 6);
        var result = Containment.Contains;

        for (int i = 0; i < planes.Length; i++)
        {
            float distance = Plane.DotNormal(plane, planes[i].Normal);

            if (distance < -planes[i].D)
            {
                return Containment.Disjoint;
            }

            if (distance < planes[i].D)
            {
                result = Containment.Intersects;
            }
        }

        return result;
    }

    public static Containment Contains(BoundingFrustum frustum, BoundingBox box)
    {
        var planes = MemoryMarshal.CreateReadOnlySpan(ref frustum.Left, 6);
        var result = Containment.Contains;

        for (int i = 0; i < planes.Length; i++)
        {
            var plane = planes[i];

            var positive = box.Minimum;
            var negative = box.Maximum;

            if (plane.Normal.X >= 0)
            {
                positive.X = box.Maximum.X;
                negative.X = box.Minimum.X;
            }

            if (plane.Normal.Y >= 0)
            {
                positive.Y = box.Maximum.Y;
                negative.Y = box.Minimum.Y;
            }

            if (plane.Normal.Z >= 0)
            {
                positive.Z = box.Maximum.Z;
                negative.Z = box.Minimum.Z;
            }

            if (Plane.DotCoordinate(plane, positive) < 0)
            {
                return Containment.Disjoint;
            }

            if (Plane.DotCoordinate(plane, negative) < 0)
            {
                result = Containment.Intersects;
            }
        }

        return result;
    }

    public static Containment Contains(BoundingFrustum a, BoundingFrustum b)
    {
        int contain = 0;
        var corners = b.GetCorners();
        var cornerSpan = MemoryMarshal.CreateReadOnlySpan(ref corners.NearTopLeft, 8);

        for (int i = 0; i < cornerSpan.Length; i++)
        {
            var point = cornerSpan[i];

            if (Contains(a, point) != Containment.Disjoint)
            {
                contain++;
            }
        }

        if (contain == 8)
        {
            return Containment.Contains;
        }

        if (contain == 0)
        {
            return Containment.Disjoint;
        }

        return Containment.Intersects;
    }

    public static Containment Contains(BoundingFrustum frustum, BoundingSphere sphere)
    {
        var planes = MemoryMarshal.CreateReadOnlySpan(ref frustum.Left, 6);
        var result = Containment.Contains;

        for (int i = 0; i < 6; i++)
        {
            float distance = Plane.DotCoordinate(planes[i], sphere.Center);

            if (distance < -sphere.Radius)
            {
                return Containment.Disjoint;
            }

            if (distance < sphere.Radius)
            {
                result = Containment.Intersects;
            }
        }

        return result;
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
