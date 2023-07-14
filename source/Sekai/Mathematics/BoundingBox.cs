// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Mathematics;

/// <summary>
/// Represents an axis-aligned bounding box in three-dimensional space.
/// </summary>
public struct BoundingBox : IContainable<BoundingBox, BoundingSphere>, IContainable<BoundingBox, BoundingBox>, IContainable<BoundingBox, Vector3>, IIntersectable<BoundingBox, BoundingSphere, bool>, IIntersectable<BoundingBox, BoundingBox, bool>, IIntersectable<BoundingBox, Plane, PlaneIntersection>, IIntersectableWithRay<BoundingBox>, IMergeable<BoundingBox>, IEquatable<BoundingBox>
{
    /// <summary>
    /// A <see cref="BoundingBox"/> that represents an empty space.
    /// </summary>
    public static readonly BoundingBox Empty = new(new(float.MaxValue), new(float.MinValue));

    /// <summary>
    /// The maximum point of the box.
    /// </summary>
    public Vector3 Minimum;

    /// <summary>
    /// The minimum point of the box.
    /// </summary>
    public Vector3 Maximum;

    /// <summary>
    /// The center of this bounding box.
    /// </summary>
    public readonly Vector3 Center => (Minimum + Maximum) / 2;

    /// <summary>
    /// The extent of this bounding box.
    /// </summary>
    public readonly Vector3 Extent => (Maximum - Minimum) / 2;

    public BoundingBox(Vector3 minimum, Vector3 maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public Corners GetCorners()
    {
        return new Corners
        {
            NearBottomLeft = new Vector3(Minimum.X, Minimum.Y, Maximum.Z),
            NearBottomRight = new Vector3(Maximum.X, Minimum.Y, Maximum.Z),
            NearTopLeft = new Vector3(Minimum.X, Maximum.Y, Maximum.Z),
            NearTopRight = new Vector3(Maximum.X, Maximum.Y, Maximum.Z),
            FarBottomLeft = new Vector3(Minimum.X, Minimum.Y, Minimum.Z),
            FarBottomRight = new Vector3(Maximum.X, Minimum.Y, Maximum.Z),
            FarTopLeft = new Vector3(Minimum.X, Maximum.Y, Maximum.Z),
            FarTopRight = new Vector3(Maximum.X, Maximum.Y, Maximum.Z),
        };
    }

    public static BoundingBox Transform(BoundingBox box, Matrix4x4 world)
    {
        var corners = box.GetCorners();
        var cornerSpan = MemoryMarshal.CreateReadOnlySpan(ref corners.NearTopLeft, 8);

        var min = Vector3.Transform(cornerSpan[0], world);
        var max = Vector3.Transform(cornerSpan[0], world);

        for (int i = 1; i < cornerSpan.Length; i++)
        {
            min = Vector3.Min(min, Vector3.Transform(cornerSpan[i], world));
            max = Vector3.Max(max, Vector3.Transform(cornerSpan[i], world));
        }

        return new BoundingBox(min, max);
    }

    public static BoundingBox FromPoints(ReadOnlySpan<Vector3> points)
    {
        var min = new Vector3(float.MinValue);
        var max = new Vector3(float.MaxValue);

        for (int i = 0; i < points.Length; i++)
        {
            min = Vector3.Min(min, points[i]);
            max = Vector3.Max(max, points[i]);
        }

        return new BoundingBox(min, max);
    }

    public static BoundingBox FromSphere(BoundingSphere sphere)
    {
        return new BoundingBox
        {
            Minimum = new Vector3(sphere.Center.X - sphere.Radius, sphere.Center.Y - sphere.Radius, sphere.Center.Z - sphere.Radius),
            Maximum = new Vector3(sphere.Center.X + sphere.Radius, sphere.Center.Y + sphere.Radius, sphere.Center.Z + sphere.Radius),
        };
    }

    public static BoundingBox Merge(BoundingBox a, BoundingBox b)
    {
        return new BoundingBox
        {
            Minimum = Vector3.Min(a.Minimum, b.Minimum),
            Maximum = Vector3.Max(a.Maximum, b.Maximum),
        };
    }

    public readonly bool Equals(BoundingBox other)
    {
        return Minimum.Equals(other.Minimum) && Maximum.Equals(other.Maximum);
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is BoundingBox box && Equals(box);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Minimum, Maximum);
    }

    public static bool Intersects(BoundingBox box, BoundingSphere sphere)
    {
        var vector = Vector3.Clamp(sphere.Center, box.Minimum, box.Maximum);
        float dist = Vector3.DistanceSquared(sphere.Center, vector);

        return dist <= sphere.Radius * sphere.Radius;
    }

    public static bool Intersects(BoundingBox a, BoundingBox b)
    {
        if (a.Minimum.X > b.Maximum.X || b.Minimum.X > a.Maximum.X)
            return false;

        if (a.Minimum.Y > b.Maximum.Y || b.Minimum.Y > a.Maximum.Y)
            return false;

        if (a.Minimum.Z > b.Maximum.Z || b.Minimum.Z > a.Maximum.Z)
            return false;

        return true;
    }

    public static bool Intersects(BoundingBox box, Ray ray, out Vector3 point)
    {
        return Ray.Intersects(ray, box, out point);
    }

    public static bool Intersects(BoundingBox box, Ray ray, out float distance)
    {
        return Ray.Intersects(ray, box, out distance);
    }

    public static bool Intersects(BoundingBox box, Ray ray)
    {
        return Ray.Intersects(ray, box);
    }

    public static Containment Contains(BoundingBox box, BoundingSphere sphere)
    {
        var vector = Vector3.Clamp(sphere.Center, box.Minimum, box.Maximum);
        float dist = Vector3.DistanceSquared(sphere.Center, vector);

        if (dist > sphere.Radius * sphere.Radius)
        {
            return Containment.Disjoint;
        }

        if ((box.Minimum.X + sphere.Radius <= sphere.Center.X) && (sphere.Center.X <= box.Maximum.X - sphere.Radius) && (box.Maximum.X - box.Minimum.X > sphere.Radius) &&
            (box.Minimum.Y + sphere.Radius <= sphere.Center.Y) && (sphere.Center.Y <= box.Maximum.Y - sphere.Radius) && (box.Maximum.Y - box.Minimum.Y > sphere.Radius) &&
            (box.Minimum.Z + sphere.Radius <= sphere.Center.Z) && (sphere.Center.Z <= box.Maximum.Z - sphere.Radius) && (box.Maximum.Z - box.Minimum.Z > sphere.Radius))
        {
            return Containment.Contains;
        }

        return Containment.Intersects;
    }

    public static Containment Contains(BoundingBox a, BoundingBox b)
    {
        if (a.Maximum.X < b.Minimum.X || a.Minimum.X > b.Maximum.X)
        {
            return Containment.Disjoint;
        }

        if (a.Maximum.Y < b.Minimum.Y || a.Minimum.Y > b.Maximum.Y)
        {
            return Containment.Disjoint;
        }

        if (a.Minimum.X <= b.Minimum.X && b.Maximum.X <= a.Maximum.X &&
            a.Minimum.Y <= b.Minimum.Y && b.Maximum.Y <= a.Maximum.Y &&
            a.Minimum.Z <= b.Minimum.Z && b.Maximum.Z <= a.Maximum.Z)
        {
            return Containment.Contains;
        }

        return Containment.Intersects;
    }

    public static Containment Contains(BoundingBox box, Vector3 point)
    {
        if (box.Minimum.X <= point.X && box.Maximum.X >= point.X &&
            box.Minimum.Y <= point.Y && box.Maximum.Y >= point.Y &&
            box.Minimum.Z <= point.Z && box.Maximum.Z >= point.Z)
        {
            return Containment.Contains;
        }

        return Containment.Disjoint;
    }

    public static PlaneIntersection Intersects(BoundingBox box, Plane plane)
    {
        return Plane.Intersects(plane, box);
    }

    public static bool operator ==(BoundingBox left, BoundingBox right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoundingBox left, BoundingBox right)
    {
        return !(left == right);
    }
}
