// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a mouse scroll wheel.
/// </summary>
public readonly struct ScrollWheel : IEquatable<ScrollWheel>
{
    /// <summary>
    /// The x-axis position of the scroll wheel.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// The y-axis position of the scroll wheel.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// Creates a new scroll wheel.
    /// </summary>
    /// <param name="x">The x-axis position of the scroll wheel.</param>
    /// <param name="y">The y-axis position of the scroll wheel.</param>
    public ScrollWheel(float x, float y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(ScrollWheel other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is ScrollWheel wheel && Equals(wheel);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(ScrollWheel left, ScrollWheel right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ScrollWheel left, ScrollWheel right)
    {
        return !(left == right);
    }
}
