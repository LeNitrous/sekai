using System;

namespace Sekai.Framework.Input;

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
}
