// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Input.Devices.Pointers;

public struct ScrollWheel : IEquatable<ScrollWheel>
{
    public float X { get; }

    public float Y { get; }

    public ScrollWheel(float x, float y)
    {
        X = x;
        Y = y;
    }

    public bool Equals(ScrollWheel other) => other.X == X && other.Y == Y;

    public override bool Equals(object? obj) => obj is ScrollWheel wheel && Equals(wheel);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(ScrollWheel left, ScrollWheel right) => left.Equals(right);

    public static bool operator !=(ScrollWheel left, ScrollWheel right) => !(left == right);
}
