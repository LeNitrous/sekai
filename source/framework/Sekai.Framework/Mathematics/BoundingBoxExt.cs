// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Mathematics;

public struct BoundingBoxExt : IEquatable<BoundingBoxExt>
{
    public static readonly BoundingBoxExt Empty = new();

    public Vector3 Maximum => Center + Extent;
    public Vector3 Minimum => Center - Extent;
    public Vector3 Center;
    public Vector3 Extent;

    public BoundingBoxExt(Vector3 center, Vector3 extent)
    {
        Center = center;
        Extent = extent;
    }

    public override bool Equals(object? obj)
    {
        return obj is BoundingBoxExt ext && Equals(ext);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Maximum, Minimum, Center, Extent);
    }

    public bool Equals(BoundingBoxExt other)
    {
        return Maximum.Equals(other.Maximum) &&
               Minimum.Equals(other.Minimum) &&
               Center.Equals(other.Center) &&
               Extent.Equals(other.Extent);
    }

    public static bool operator ==(BoundingBoxExt left, BoundingBoxExt right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoundingBoxExt left, BoundingBoxExt right)
    {
        return !(left == right);
    }

    public static explicit operator BoundingBox(BoundingBoxExt ext)
    {
        return new BoundingBox(ext.Minimum, ext.Maximum);
    }

    public static explicit operator BoundingBoxExt(BoundingBox box)
    {
        return new BoundingBoxExt(box.Center, box.Extent);
    }
}
