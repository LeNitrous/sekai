// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Audio;

/// <summary>
/// Listener orientation.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ListenerOrientation : IEquatable<ListenerOrientation>
{
    /// <summary>
    /// The default listener orientation.
    /// </summary>
    public static ListenerOrientation Default => new();

    /// <summary>
    /// The "at" vector.
    /// </summary>
    public Vector3 At;

    /// <summary>
    /// The "up" vector.
    /// </summary>
    public Vector3 Up;

    public ListenerOrientation(Vector3 at, Vector3 up)
    {
        At = at;
        Up = up;
    }

    public override bool Equals(object? obj)
    {
        return obj is ListenerOrientation orientation && Equals(orientation);
    }

    public bool Equals(ListenerOrientation other)
    {
        return At.Equals(other.At) &&  Up.Equals(other.Up);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(At, Up);
    }

    public static bool operator ==(ListenerOrientation left, ListenerOrientation right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ListenerOrientation left, ListenerOrientation right)
    {
        return !(left == right);
    }
}

