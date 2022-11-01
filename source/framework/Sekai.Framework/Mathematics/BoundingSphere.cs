// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Mathematics;

public struct BoundingSphere : IEquatable<BoundingSphere>
{
    public static readonly BoundingSphere Empty = new();

    public Vector3 Center;
    public float Radius;

    public BoundingSphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public override bool Equals(object? obj)
    {
        return obj is BoundingSphere sphere && Equals(sphere);
    }

    public bool Equals(BoundingSphere other)
    {
        return Center.Equals(other.Center) &&
               Radius == other.Radius;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Center, Radius);
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
