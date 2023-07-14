// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Mathematics;

/// <summary>
/// A set of points that define a primitive's corners.
/// </summary>
public struct Corners : IEquatable<Corners>
{
    /// <summary>
    /// The near plane's top left corner.
    /// </summary>
    public Vector3 NearTopLeft;

    /// <summary>
    /// The near plane's top right corner.
    /// </summary>
    public Vector3 NearTopRight;

    /// <summary>
    /// The near plane's bottom left corner.
    /// </summary>
    public Vector3 NearBottomLeft;

    /// <summary>
    /// The near plane's bottom right corner.
    /// </summary>
    public Vector3 NearBottomRight;

    /// <summary>
    /// The far plane's top left corner.
    /// </summary>
    public Vector3 FarTopLeft;

    /// <summary>
    /// The far plane's top right corner.
    /// </summary>
    public Vector3 FarTopRight;

    /// <summary>
    /// The far plane's bottom left corner.
    /// </summary>
    public Vector3 FarBottomLeft;

    /// <summary>
    /// The far plane's bottom right corner.
    /// </summary>
    public Vector3 FarBottomRight;

    public Corners(Vector3 nearTopLeft, Vector3 nearTopRight, Vector3 nearBottomLeft, Vector3 nearBottomRight, Vector3 farTopLeft, Vector3 farTopRight, Vector3 farBottomLeft, Vector3 farBottomRight)
    {
        NearTopLeft = nearTopLeft;
        NearTopRight = nearTopRight;
        NearBottomLeft = nearBottomLeft;
        NearBottomRight = nearBottomRight;
        FarTopLeft = farTopLeft;
        FarTopRight = farTopRight;
        FarBottomLeft = farBottomLeft;
        FarBottomRight = farBottomRight;
    }

    public readonly bool Equals(Corners other)
    {
        return NearTopLeft.Equals(other.NearTopLeft) &&
               NearTopRight.Equals(other.NearTopRight) &&
               NearBottomLeft.Equals(other.NearBottomLeft) &&
               NearBottomRight.Equals(other.NearBottomRight) &&
               FarTopLeft.Equals(other.FarTopLeft) &&
               FarTopRight.Equals(other.FarTopRight) &&
               FarBottomLeft.Equals(other.FarBottomLeft) &&
               FarBottomRight.Equals(other.FarBottomRight);
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Corners corners && Equals(corners);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(NearTopLeft, NearTopRight, NearBottomLeft, NearBottomRight, FarTopLeft, FarTopRight, FarBottomLeft, FarBottomRight);
    }

    public static bool operator ==(Corners left, Corners right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Corners left, Corners right)
    {
        return !(left == right);
    }
}
