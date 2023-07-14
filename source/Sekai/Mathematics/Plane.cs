// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Represents a plane in three dimensional space.
/// </summary>
public struct Plane : IIntersectable<Plane, BoundingSphere, PlaneIntersection>, IIntersectable<Plane, BoundingBox, PlaneIntersection>, IIntersectable<Plane, Plane, bool>, IIntersectable<Plane, Vector3, PlaneIntersection>, IIntersectableWithRay<Plane>, IEquatable<Plane>
{
    /// <summary>
    /// The plane's normal vector.
    /// </summary>
    public Vector3 Normal;

    /// <summary>
    /// The plane's distance along its normal from the origin.
    /// </summary>
    public float D;

    public Plane(float value)
    {
        D = value;
        Normal = new Vector3(value);
    }

    public Plane(float x, float y, float z, float d)
    {
        D = d;
        Normal = new Vector3(x, y, z);
    }

    public Plane(Vector4 value)
        : this(value.X, value.Y, value.Z, value.W)
    {
    }

    public Plane(Vector3 point, Vector3 normal)
    {
        D = Vector3.Dot(normal, point);
        Normal = normal;
    }

    public Plane(Vector3 normal, float d)
    {
        D = d;
        Normal = normal;
    }

    public readonly bool Equals(Plane other)
    {
        return Normal.Equals(other.Normal) && D.Equals(other.D);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Plane plane && Equals(plane);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Normal, D);
    }

    /// <summary>
    /// Create a <see cref="Plane"/> from vertices.
    /// </summary>
    /// <param name="point1">The first point.</param>
    /// <param name="point2">The second point.</param>
    /// <param name="point3">The third point.</param>
    /// <returns>A new <see cref="Plane"/>.</returns>
    public static Plane CreateFromVertices(Vector3 point1, Vector3 point2, Vector3 point3)
    {
        Plane plane = default;

        float x1 = point2.X - point1.X;
        float y1 = point2.Y - point1.Y;
        float z1 = point2.Z - point1.Z;
        float x2 = point3.X - point1.X;
        float y2 = point3.Y - point1.Y;
        float z2 = point3.Z - point1.Z;
        float yz = (y1 * z2) - (z1 * y2);
        float xz = (z1 * x2) - (x1 * z2);
        float xy = (x1 * y2) - (y1 * x2);
        float invPyth = 1.0f / MathF.Sqrt((yz * yz) + (xz * xz) + (xy * xy));

        plane.Normal.X = yz * invPyth;
        plane.Normal.Y = xz * invPyth;
        plane.Normal.Z = xy * invPyth;
        plane.D = -((plane.Normal.X * point1.X) + (plane.Normal.Y * point1.Y) + (plane.Normal.Z * point1.Z));

        return plane;
    }

    /// <summary>
    /// Returns the dot product of a plane and a 4-dimensional vector.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="value">The vector.</param>
    /// <returns>The result of the dot product.</returns>
    public static float Dot(Plane plane, Vector4 value)
    {
        return (plane.Normal.X * value.X) + (plane.Normal.Y * value.Y) + (plane.Normal.Z * value.Z) + (plane.D * value.W);
    }

    /// <summary>
    /// Returns the dot product of a specified three-dimensional vector and the normal vector of this plane plus the distance value of the plane.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="value">The vector.</param>
    /// <returns>The result of the dot product.</returns>
    public static float DotCoordinate(Plane plane, Vector3 value)
    {
        return (plane.Normal.X * value.X) + (plane.Normal.Y * value.Y) + (plane.Normal.Z * value.Z) + plane.D;
    }

    /// <summary>
    /// Returns the dot product of a specified three-dimensional vector and the normal vector of this plane.
    /// </summary>
    /// <param name="plane">The plane.</param>
    /// <param name="value">The vector.</param>
    /// <returns>The result of the dot product.</returns>
    public static float DotNormal(Plane plane, Vector3 value)
    {
        return (plane.Normal.X * value.X) + (plane.Normal.Y * value.Y) + (plane.Normal.Z * value.Z);
    }

    /// <summary>
    /// Creates a new plane whose normal vector is the source plane's normal vector normalized.
    /// </summary>
    /// <param name="plane">The plane to normalize.</param>
    /// <returns>A new plane that is normalized.</returns>
    public static Plane Normalize(Plane plane)
    {
        Plane result = default;

        float magnitude = 1.0f / MathF.Sqrt((plane.Normal.X * plane.Normal.X) + (plane.Normal.Y * plane.Normal.Y) + (plane.Normal.Z * plane.Normal.Z));

        result.Normal.X = plane.Normal.X * magnitude;
        result.Normal.Y = plane.Normal.Y * magnitude;
        result.Normal.Z = plane.Normal.Z * magnitude;
        result.D = plane.D * magnitude;

        return result;
    }

    /// <summary>
    /// Transforms a normalized plane b y a 4x4 matrix.
    /// </summary>
    /// <param name="plane">The plane to transform.</param>
    /// <param name="matrix">The matrix to use in transformation.</param>
    /// <returns>A transformed plane.</returns>
    public static Plane Transform(Plane plane, Matrix4x4 matrix)
    {
        Plane result = default;

        float x = plane.Normal.X;
        float y = plane.Normal.Y;
        float z = plane.Normal.Z;
        float d = plane.D;

        Matrix4x4.Invert(matrix, out var transformation);

        result.Normal.X = (x * transformation.M11) + (y * transformation.M12) + (z * transformation.M13) + (d * transformation.M14);
        result.Normal.Y = (x * transformation.M21) + (y * transformation.M22) + (z * transformation.M23) + (d * transformation.M24);
        result.Normal.Z = (x * transformation.M31) + (y * transformation.M32) + (z * transformation.M33) + (d * transformation.M34);
        result.D = (x * transformation.M41) + (y * transformation.M42) + (z * transformation.M43) + (d * transformation.M44);

        return result;
    }

    /// <summary>
    /// Transforms a normalized plane by a quaternion rotation.
    /// </summary>
    /// <param name="plane">The plane to transform.</param>
    /// <param name="rotation">The quaternion to use in transformation.</param>
    /// <returns>A transformed plane.</returns>
    public static Plane Transform(Plane plane, Quaternion rotation)
    {
        Plane result = default;

        float x2 = rotation.X + rotation.X;
        float y2 = rotation.Y + rotation.Y;
        float z2 = rotation.Z + rotation.Z;
        float wx = rotation.W * x2;
        float wy = rotation.W * y2;
        float wz = rotation.W * z2;
        float xx = rotation.X * x2;
        float xy = rotation.X * y2;
        float xz = rotation.X * z2;
        float yy = rotation.Y * y2;
        float yz = rotation.Y * z2;
        float zz = rotation.Z * z2;

        float x = plane.Normal.X;
        float y = plane.Normal.Y;
        float z = plane.Normal.Z;

        result.Normal.X = (x * (1.0f - yy - zz)) + (y * (xy - wz)) + (z * (xz + wy));
        result.Normal.Y = (x * (xy + wz)) + (y * (1.0f - xx - zz)) + (z * (yz - wx));
        result.Normal.Z = (x * (xz - wy)) + (y * (yz + wx)) + (z * (1.0f - xx - yy));
        result.D = plane.D;

        return result;
    }

    public static PlaneIntersection Intersects(Plane plane, BoundingSphere sphere)
    {
        float distance = Vector3.Dot(plane.Normal, sphere.Center);
        distance += plane.D;

        if (distance > sphere.Radius)
            return PlaneIntersection.Front;

        if (distance < -sphere.Radius)
            return PlaneIntersection.Back;

        return PlaneIntersection.Intersects;
    }

    public static PlaneIntersection Intersects(Plane plane, BoundingBox box)
    {
        Vector3 min;
        Vector3 max;

        max.X = (plane.Normal.X >= 0.0f) ? box.Minimum.X : box.Maximum.X;
        max.Y = (plane.Normal.Y >= 0.0f) ? box.Minimum.Y : box.Maximum.Y;
        max.Z = (plane.Normal.Z >= 0.0f) ? box.Minimum.Z : box.Maximum.Z;
        min.X = (plane.Normal.X >= 0.0f) ? box.Maximum.X : box.Minimum.X;
        min.Y = (plane.Normal.Y >= 0.0f) ? box.Maximum.Y : box.Minimum.Y;
        min.Z = (plane.Normal.Z >= 0.0f) ? box.Maximum.Z : box.Minimum.Z;

        float distance = Vector3.Dot(plane.Normal, max);

        if (distance + plane.D > 0.0f)
            return PlaneIntersection.Front;

        distance = Vector3.Dot(plane.Normal, min);

        if (distance + plane.D < 0.0f)
            return PlaneIntersection.Back;

        return PlaneIntersection.Intersects;
    }

    public static bool Intersects(Plane a, Plane b)
    {
        var direction = Vector3.Cross(a.Normal, b.Normal);
        float denominator = Vector3.Dot(direction, direction);

        if (MathF.Abs(denominator) < float.Epsilon)
        {
            return false;
        }

        return true;
    }

    public static bool Intersects(Plane a, Plane b, out Ray line)
    {
        var direction = Vector3.Cross(a.Normal, b.Normal);
        float denominator = Vector3.Dot(direction, direction);

        if (MathF.Abs(denominator) < float.Epsilon)
        {
            line = default;
            return false;
        }

        var temp = (a.D * b.Normal) - (b.D * a.Normal);
        var point = Vector3.Cross(temp, direction);

        line.Origin = point;
        line.Direction = Vector3.Normalize(direction);

        return true;
    }

    public static PlaneIntersection Intersects(Plane plane, Vector3 point)
    {
        float distance = Vector3.Dot(plane.Normal, point) + plane.D;

        if (distance > 0f)
        {
            return PlaneIntersection.Front;
        }

        if (distance < 0f)
        {
            return PlaneIntersection.Back;
        }

        return PlaneIntersection.Intersects;
    }

    public static bool Intersects(Plane plane, Ray ray, out Vector3 point)
    {
        return Ray.Intersects(ray, plane, out point);
    }

    public static bool Intersects(Plane plane, Ray ray, out float distance)
    {
        return Ray.Intersects(ray, plane, out distance);
    }

    public static bool Intersects(Plane plane, Ray ray)
    {
        return Ray.Intersects(ray, plane);
    }

    public static bool operator ==(Plane left, Plane right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Plane left, Plane right)
    {
        return !(left == right);
    }
}
