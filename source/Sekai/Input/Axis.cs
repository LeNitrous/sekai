// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Input;

/// <summary>
/// Represents a controller axis.
/// </summary>
public readonly struct Axis : IEquatable<Axis>
{
    /// <summary>
    /// The axis index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The axis position.
    /// </summary>
    public float Position { get; }

    /// <summary>
    /// Creates a new axis.
    /// </summary>
    /// <param name="index">The axis index.</param>
    /// <param name="position">The axis position.</param>
    public Axis(int index, float position)
    {
        Index = index;
        Position = position;
    }

    public bool Equals(Axis other)
    {
        return Index.Equals(other.Index) && Position.Equals(other.Position);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Axis axis && Equals(axis);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, Position);
    }

    public static bool operator ==(Axis left, Axis right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Axis left, Axis right)
    {
        return !(left == right);
    }
}
