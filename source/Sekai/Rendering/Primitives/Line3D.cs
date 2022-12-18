// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A line in three-dimensional space.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Line3D : IEquatable<Line3D>
{
    /// <summary>
    /// The start point.
    /// </summary>
    public Vector3 Start;

    /// <summary>
    /// The end point.
    /// </summary>
    public Vector3 End;

    public Line3D(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }

    public override bool Equals(object? obj)
    {
        return obj is Line3D d && Equals(d);
    }

    public bool Equals(Line3D other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public static bool operator ==(Line3D left, Line3D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Line3D left, Line3D right)
    {
        return !(left == right);
    }
}
