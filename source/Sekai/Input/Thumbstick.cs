// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Input;

/// <summary>
/// Represents a controller thumbstick.
/// </summary>
public readonly struct Thumbstick : IEquatable<Thumbstick>
{
    /// <summary>
    /// The index of the stick.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The x-axis of the stick, from -1.0 to 1.0.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// The y-axis of the stick, from -1.0 to 1.0.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// The current position of the stick, from 0.0 to 1.0.
    /// </summary>
    public float Position => MathF.Sqrt((X * X) + (Y * Y));

    /// <summary>
    /// The current direction of the stick, from -π to π.
    /// </summary>
    public float Direction => MathF.Atan2(Y, X);

    /// <summary>
    /// Creates a new thumbstick.
    /// </summary>
    /// <param name="index">The thumbstick index.</param>
    /// <param name="x">The x-axis position of the stick.</param>
    /// <param name="y">The y-axis position of the stick.</param>
    public Thumbstick(int index, float x, float y)
    {
        Index = index;
        X = x;
        Y = y;
    }

    public bool Equals(Thumbstick other)
    {
        return Index.Equals(other.Index) && X.Equals(other.X) && Y.Equals(other.Y);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Thumbstick stick && Equals(stick);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, X, Y);
    }

    public static bool operator ==(Thumbstick left, Thumbstick right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Thumbstick left, Thumbstick right)
    {
        return !(left == right);
    }
}
