// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a controller trigger.
/// </summary>
public readonly struct Trigger : IEquatable<Trigger>
{
    /// <summary>
    /// The trigger index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The trigger position; how far it is currently pressed down.
    /// </summary>
    public float Position { get; }

    /// <summary>
    /// Creates a new trigger.
    /// </summary>
    /// <param name="index">The index of the trigger.</param>
    /// <param name="position">The position of the trigger.</param>
    public Trigger(int index, float position)
    {
        Index = index;
        Position = position;
    }

    public bool Equals(Trigger other)
    {
        return Index.Equals(other.Index) && Position.Equals(other.Position);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Trigger trigger && Equals(trigger);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Index, Position);
    }

    public static bool operator ==(Trigger left, Trigger right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Trigger left, Trigger right)
    {
        return !(left == right);
    }
}
