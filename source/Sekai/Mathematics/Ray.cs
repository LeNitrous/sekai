// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Represents a three dimensional line based on a point in space and a direction.
/// </summary>
public struct Ray : IIntersectable<Ray, Ray, bool>, IIntersectable<Ray, Vector3, bool>, IEquatable<Ray>
{
    /// <summary>
    /// The position where the ray starts.
    /// </summary>
    public Vector3 Origin;

    /// <summary>
    /// The normalized direction where the ray points.
    /// </summary>
    public Vector3 Direction;

    public Ray(Vector3 position, Vector3 direction)
    {
        Origin = position;
        Direction = direction;
    }

    public readonly bool Equals(Ray other)
    {
        return Origin.Equals(other.Origin) && Direction.Equals(other.Direction);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Ray ray && Equals(ray);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Origin, Direction);
    }

    public static Ray Transform(Ray ray, Matrix4x4 world)
    {
        return new Ray(Vector3.Transform(ray.Origin, world), Vector3.Normalize(Vector3.Transform(ray.Direction, world)));
    }

    public static bool Intersects(Ray ray, Vector3 point)
    {
        var m = Vector3.Subtract(ray.Origin, point);

        float b = Vector3.Dot(m, ray.Direction);
        float c = Vector3.Dot(m, m) - float.Epsilon;

        if (c > 0f && b > 0f)
        {
            return false;
        }

        float discriminant = (b * b) - c;

        if (discriminant < 0f)
        {
            return false;
        }

        return true;
    }

    public static bool Intersects(Ray a, Ray b, out Vector3 point)
    {
        var cross = Vector3.Cross(a.Direction, b.Direction);
        float denominator = cross.Length();

        if (MathF.Abs(denominator) < float.Epsilon)
        {
            if (MathF.Abs(b.Origin.X - a.Origin.X) < float.Epsilon &&
                MathF.Abs(b.Origin.Y - a.Origin.Y) < float.Epsilon &&
                MathF.Abs(b.Origin.Z - a.Origin.Z) < float.Epsilon)
            {
                point = Vector3.Zero;
                return true;
            }
        }

        denominator *= denominator;

        float m11 = b.Origin.X - a.Origin.X;
        float m12 = b.Origin.Y - a.Origin.Y;
        float m13 = b.Origin.Z - a.Origin.Z;
        float m21 = b.Direction.X;
        float m22 = b.Direction.Y;
        float m23 = b.Direction.Z;
        float m31 = cross.X;
        float m32 = cross.Y;
        float m33 = cross.Z;

        float dets =
            (m11 * m22 * m33) +
            (m12 * m23 * m31) +
            (m13 * m21 * m32) -
            (m11 * m23 * m32) -
            (m12 * m21 * m33) -
            (m13 * m22 * m31);

        m21 = a.Direction.X;
        m22 = a.Direction.Y;
        m23 = a.Direction.Z;

        float dett =
            (m11 * m22 * m33) +
            (m12 * m23 * m31) +
            (m13 * m21 * m32) -
            (m11 * m23 * m32) -
            (m12 * m21 * m33) -
            (m13 * m22 * m31);

        float s = dets / denominator;
        float t = dett / denominator;

        var pointA = a.Origin + (s * a.Direction);
        var pointB = b.Origin + (t * b.Direction);

        if (MathF.Abs(pointB.X - pointA.X) > float.Epsilon ||
            MathF.Abs(pointB.Y - pointA.Y) > float.Epsilon ||
            MathF.Abs(pointB.Z - pointA.Z) > float.Epsilon)
        {
            point = Vector3.Zero;
            return false;
        }

        point = pointA;
        return true;
    }

    public static bool Intersects(Ray left, Ray right)
    {
        return Intersects(left, right, out _);
    }

    public static bool Intersects(Ray ray, Plane plane)
    {
        return Intersects(ray, plane, out float _);
    }

    public static bool Intersects(Ray ray, Plane plane, out float distance)
    {
        float direction = Vector3.Dot(plane.Normal, ray.Direction);

        if (MathF.Abs(direction) < -float.Epsilon)
        {
            distance = 0f;
            return false;
        }

        float position = Vector3.Dot(plane.Normal, ray.Origin);
        distance = (-plane.D - position) / direction;

        if (distance < 0f)
        {
            if (distance < -float.Epsilon)
            {
                distance = 0;
                return false;
            }

            distance = 0f;
        }

        return true;
    }

    public static bool Intersects(Ray ray, Plane plane, out Vector3 point)
    {
        if (!Intersects(ray, plane, out float distance))
        {
            point = Vector3.Zero;
            return false;
        }

        point = ray.Origin + (ray.Direction * distance);
        return true;
    }

    public static bool Intersects(Ray ray, BoundingBox box)
    {
        return Intersects(ray, box, out float _);
    }

    public static bool Intersects(Ray ray, BoundingBox box, out float distance)
    {
        distance = 0f;
        float tmax = float.MaxValue;

        if (MathF.Abs(ray.Direction.X) < float.Epsilon)
        {
            if (ray.Origin.X < box.Minimum.X || ray.Origin.X > box.Maximum.X)
            {
                distance = 0f;
                return false;
            }
        }
        else
        {
            float inverse = 1.0f / ray.Direction.X;
            float t1 = (box.Minimum.X - ray.Origin.X) * inverse;
            float t2 = (box.Maximum.X - ray.Origin.X) * inverse;

            if (t1 > t2)
            {
                (t2, t1) = (t1, t2);
            }

            distance = MathF.Max(t1, distance);
            tmax = MathF.Min(t2, tmax);

            if (distance > tmax)
            {
                distance = 0f;
                return false;
            }
        }

        if (MathF.Abs(ray.Direction.Y) < float.Epsilon)
        {
            if (ray.Origin.Y < box.Minimum.Y || ray.Origin.Y > box.Maximum.Y)
            {
                distance = 0f;
                return false;
            }
        }
        else
        {
            float inverse = 1.0f / ray.Direction.Y;
            float t1 = (box.Minimum.Y - ray.Origin.Y) * inverse;
            float t2 = (box.Maximum.Y - ray.Origin.Y) * inverse;

            if (t1 > t2)
            {
                (t2, t1) = (t1, t2);
            }

            distance = MathF.Max(t1, distance);
            tmax = MathF.Min(t2, tmax);

            if (distance > tmax)
            {
                distance = 0f;
                return false;
            }
        }

        if (MathF.Abs(ray.Direction.Z) < float.Epsilon)
        {
            if (ray.Origin.Z < box.Minimum.Z || ray.Origin.Z > box.Maximum.Z)
            {
                distance = 0f;
                return false;
            }
        }
        else
        {
            float inverse = 1.0f / ray.Direction.Z;
            float t1 = (box.Minimum.Z - ray.Origin.Z) * inverse;
            float t2 = (box.Maximum.Z - ray.Origin.Z) * inverse;

            if (t1 > t2)
            {
                (t2, t1) = (t1, t2);
            }

            distance = MathF.Max(t1, distance);
            tmax = MathF.Min(t2, tmax);

            if (distance > tmax)
            {
                distance = 0f;
                return false;
            }
        }

        return true;
    }

    public static bool Intersects(Ray ray, BoundingBox box, out Vector3 point)
    {
        if (!Intersects(ray, box, out float distance))
        {
            point = Vector3.Zero;
            return false;
        }

        point = ray.Origin + (ray.Direction * distance);
        return true;
    }

    public static bool Intersects(Ray ray, BoundingSphere sphere)
    {
        return Intersects(ray, sphere, out float _);
    }

    public static bool Intersects(Ray ray, BoundingSphere sphere, out float distance)
    {
        var m = Vector3.Subtract(ray.Origin, sphere.Center);

        float b = Vector3.Dot(m, ray.Direction);
        float c = Vector3.Dot(m, m) - (sphere.Radius * sphere.Radius);

        if (c > 0f && b > 0f)
        {
            distance = 0f;
            return false;
        }

        float discriminant = (b * b) - c;

        if (discriminant < 0f)
        {
            distance = 0f;
            return false;
        }

        distance = -b - MathF.Sqrt(discriminant);

        if (distance < 0f)
            distance = 0f;

        return true;
    }

    public static bool Intersects(Ray ray, BoundingSphere sphere, out Vector3 point)
    {
        if (!Intersects(ray, sphere, out float distance))
        {
            point = Vector3.Zero;
            return false;
        }

        point = ray.Origin + (ray.Direction * distance);
        return true;
    }

    public static bool operator ==(Ray left, Ray right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Ray left, Ray right)
    {
        return !(left == right);
    }
}
