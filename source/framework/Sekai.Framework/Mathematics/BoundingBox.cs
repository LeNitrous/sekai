// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Mathematics;

public struct BoundingBox : IEquatable<BoundingBox>
{
    public static readonly BoundingBox Empty = new();

    public Vector3 Minimum;
    public Vector3 Maximum;
    public Vector3 Center => (Maximum + Minimum) / 2.0f;
    public Vector3 Extent => (Maximum - Minimum) / 2.0f;

    public BoundingBox(Vector3 minimum, Vector3 maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
    }

    public override bool Equals(object? obj)
    {
        return obj is BoundingBox box && Equals(box);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Minimum, Maximum, Center, Extent);
    }

    public bool Equals(BoundingBox other)
    {
        return Minimum.Equals(other.Minimum) &&
               Maximum.Equals(other.Maximum) &&
               Center.Equals(other.Center) &&
               Extent.Equals(other.Extent);
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
