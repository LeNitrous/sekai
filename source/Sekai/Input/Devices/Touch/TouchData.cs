// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Input.Devices.Touch;

/// <summary>
/// Represents information about a touch event.
/// </summary>
public readonly struct TouchData : IEquatable<TouchData>
{
    /// <summary>
    /// The position of the touch.
    /// </summary>
    public readonly Vector2 Position;

    /// <summary>
    /// The source touch.
    /// </summary>
    public readonly TouchSource Source;

    public TouchData(TouchSource source, Vector2 position)
    {
        Source = source;
        Position = position;
    }
    public bool Equals(TouchData other)
    {
        return Source == other.Source && Position.Equals(other.Position);
    }

    public override bool Equals(object? obj)
    {
        return obj is TouchData touch && Equals(touch);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source, Position);
    }

    public static bool operator ==(TouchData left, TouchData right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TouchData left, TouchData right)
    {
        return !(left == right);
    }
}
