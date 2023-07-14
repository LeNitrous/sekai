// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// Represents a bounding sphere in three dimensional space.
/// </summary>
public struct BoundingSphere : IContainable<BoundingSphere, BoundingSphere>, IContainable<BoundingSphere, BoundingBox>, IContainable<BoundingSphere, Vector3>, IIntersectable<BoundingSphere, BoundingSphere, bool>, IIntersectable<BoundingSphere, BoundingBox, bool>, IIntersectable<BoundingSphere, Plane, PlaneIntersection>, IIntersectableWithRay<BoundingSphere>, IMergeable<BoundingSphere>, IEquatable<BoundingSphere>
{
    /// <summary>
    /// An empty bounding sphere.
    /// </summary>
    public static readonly BoundingSphere Empty = new();

    /// <summary>
    /// The center of the sphere.
    /// </summary>
    public Vector3 Center;

    /// <summary>
    /// The radius of the sphere.
    /// </summary>
    public float Radius;

    public BoundingSphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public readonly bool Equals(BoundingSphere other)
    {
        return Center.Equals(other.Center) && Radius.Equals(other.Radius);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is BoundingSphere sphere && Equals(sphere);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Center, Radius);
    }

    public static BoundingSphere Transform(BoundingSphere sphere, Matrix4x4 world)
    {
        var center = Vector3.Transform(sphere.Center, world);

        float majorAxisLengthSquared = MathF.Max(
            (world.M11 * world.M11) + (world.M12 * world.M12) + (world.M13 * world.M13), MathF.Max(
                (world.M21 * world.M21) + (world.M22 * world.M22) + (world.M23 * world.M23),
                (world.M31 * world.M31) + (world.M32 * world.M32) + (world.M33 * world.M33)));

        float radius = sphere.Radius * MathF.Sqrt(majorAxisLengthSquared);

        return new BoundingSphere(center, radius);
    }

    public static BoundingSphere Merge(BoundingSphere a, BoundingSphere b)
    {
        if (a == Empty)
        {
            return a;
        }

        if (b == Empty)
        {
            return b;
        }

        var diff = b.Center - a.Center;
        float length = diff.Length();

        if (a.Radius + b.Radius >= length)
        {
            if (a.Radius - b.Radius >= length)
            {
                return a;
            }

            if (b.Radius - a.Radius >= length)
            {
                return b;
            }
        }

        var vector = diff * (1.0f / length);
        float min = MathF.Min(-a.Radius, length - b.Radius);
        float max = (MathF.Max(a.Radius, length + b.Radius) - min) * 0.5f;

        return new BoundingSphere
        {
            Center = a.Center + (vector * (max + min)),
            Radius = max
        };
    }

    public static BoundingSphere FromBox(BoundingBox box)
    {
        var center = Vector3.Lerp(box.Minimum, box.Maximum, 0.5f);

        float x = box.Minimum.X - box.Maximum.X;
        float y = box.Minimum.Y - box.Maximum.Y;
        float z = box.Minimum.Z - box.Maximum.Z;

        float dist = MathF.Sqrt((x * x) + (y * y) + (z * z));

        return new BoundingSphere(center, dist * 0.5f);
    }

    public static BoundingSphere FromPoints(ReadOnlySpan<Vector3> points)
    {
        var center = Vector3.Zero;

        for (int i = 0; i < points.Length; i++)
        {
            center = Vector3.Add(points[i], center);
        }

        center /= points.Length;

        float radius = 0f;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.DistanceSquared(center, points[i]);

            if (distance > radius)
            {
                radius = distance;
            }
        }

        radius = MathF.Sqrt(radius);

        return new BoundingSphere(center, radius);
    }

    public static bool Intersects(BoundingSphere sphere, Ray ray, out Vector3 point)
    {
        return Ray.Intersects(ray, sphere, out point);
    }

    public static bool Intersects(BoundingSphere sphere, Ray ray, out float distance)
    {
        return Ray.Intersects(ray, sphere, out distance);
    }

    public static bool Intersects(BoundingSphere sphere, Ray ray)
    {
        return Ray.Intersects(ray, sphere);
    }

    public static bool Intersects(BoundingSphere a, BoundingSphere b)
    {
        float radiisum = a.Radius + b.Radius;
        return Vector3.DistanceSquared(a.Center, b.Center) <= radiisum * radiisum;
    }

    public static Containment Contains(BoundingSphere a, BoundingSphere b)
    {
        float distance = Vector3.Distance(a.Center, b.Center);

        if (a.Radius + b.Radius < distance)
            return Containment.Disjoint;

        if (a.Radius - b.Radius < distance)
            return Containment.Intersects;

        return Containment.Contains;
    }

    public static Containment Contains(BoundingSphere sphere, BoundingBox box)
    {
        Vector3 vector;

        if (!BoundingBox.Intersects(box, sphere))
            return Containment.Disjoint;

        float radiussquared = sphere.Radius * sphere.Radius;
        vector.X = sphere.Center.X - box.Minimum.X;
        vector.Y = sphere.Center.Y - box.Maximum.Y;
        vector.Z = sphere.Center.Z - box.Maximum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Maximum.X;
        vector.Y = sphere.Center.Y - box.Maximum.Y;
        vector.Z = sphere.Center.Z - box.Maximum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Maximum.X;
        vector.Y = sphere.Center.Y - box.Minimum.Y;
        vector.Z = sphere.Center.Z - box.Maximum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Minimum.X;
        vector.Y = sphere.Center.Y - box.Minimum.Y;
        vector.Z = sphere.Center.Z - box.Maximum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Minimum.X;
        vector.Y = sphere.Center.Y - box.Maximum.Y;
        vector.Z = sphere.Center.Z - box.Minimum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Maximum.X;
        vector.Y = sphere.Center.Y - box.Maximum.Y;
        vector.Z = sphere.Center.Z - box.Minimum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Maximum.X;
        vector.Y = sphere.Center.Y - box.Minimum.Y;
        vector.Z = sphere.Center.Z - box.Minimum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        vector.X = sphere.Center.X - box.Minimum.X;
        vector.Y = sphere.Center.Y - box.Minimum.Y;
        vector.Z = sphere.Center.Z - box.Minimum.Z;

        if (vector.LengthSquared() > radiussquared)
            return Containment.Intersects;

        return Containment.Contains;
    }

    public static Containment Contains(BoundingSphere sphere, Vector3 point)
    {
        if (Vector3.DistanceSquared(point, sphere.Center) <= sphere.Radius * sphere.Radius)
        {
            return Containment.Contains;
        }

        return Containment.Disjoint;
    }

    public static bool Intersects(BoundingSphere sphere, BoundingBox box)
    {
        return BoundingBox.Intersects(box, sphere);
    }

    public static PlaneIntersection Intersects(BoundingSphere sphere, Plane plane)
    {
        return Plane.Intersects(plane, sphere);
    }

    public static bool operator ==(BoundingSphere left, BoundingSphere right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoundingSphere left, BoundingSphere right)
    {
        return !(left == right);
    }
}
