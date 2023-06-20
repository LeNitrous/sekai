// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a controller hat.
/// </summary>
public readonly struct Hat : IEquatable<Hat>
{
    /// <summary>
    /// The hat's index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The hat's position.
    /// </summary>
    public HatPosition Position { get; }

    /// <summary>
    /// Creates a new hat.
    /// </summary>
    /// <param name="index">The hat index.</param>
    /// <param name="position">The hat position.</param>
    public Hat(int index, HatPosition position)
    {
        Index = index;
        Position = position;
    }

    public bool Equals(Hat other)
    {
        return Index.Equals(other.Index) && Position == other.Position;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Hat hat && Equals(hat);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, Position);
    }

    public static bool operator ==(Hat left, Hat right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Hat left, Hat right)
    {
        return !(left == right);
    }
}
