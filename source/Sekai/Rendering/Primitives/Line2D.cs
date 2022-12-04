// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A line in two-dimensional space.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Line2D : IPrimitive<Vector2>, IEquatable<Line2D>
{
    /// <summary>
    /// The start point.
    /// </summary>
    public Vector2 Start;

    /// <summary>
    /// The end point.
    /// </summary>
    public Vector2 End;

    public Line2D(Vector2 start, Vector2 end)
    {
        Start = start;
        End = end;
    }

    public override bool Equals(object? obj)
    {
        return obj is Line2D d && Equals(d);
    }

    public bool Equals(Line2D other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public ReadOnlySpan<Vector2> GetPoints() => MemoryMarshal.CreateReadOnlySpan(ref Unsafe.AsRef(in Start), 2);

    public static bool operator ==(Line2D left, Line2D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Line2D left, Line2D right)
    {
        return !(left == right);
    }
}
