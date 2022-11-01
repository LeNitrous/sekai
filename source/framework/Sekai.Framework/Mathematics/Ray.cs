// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Mathematics;

public struct Ray : IEquatable<Ray>
{
    public Vector3 Origin;
    public Vector3 Direction;

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = direction;
    }

    public override bool Equals(object? obj)
    {
        return obj is Ray ray && Equals(ray);
    }

    public bool Equals(Ray other)
    {
        return Origin.Equals(other.Origin) &&
               Direction.Equals(other.Direction);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Origin, Direction);
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
