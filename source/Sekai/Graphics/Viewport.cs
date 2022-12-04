// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Graphics;

/// <summary>
/// Describes a 3-dimensional region.
/// </summary>
public readonly struct Viewport : IEquatable<Viewport>
{
    /// <summary>
    /// Represents an empty <see cref="Viewport">.
    /// </summary>
    public static Viewport Empty => new(Rectangle.Empty, 0, 0);

    /// <summary>
    /// The viewport X position.
    /// </summary>
    public int X => Rectangle.X;

    /// <summary>
    /// The viewport Y position.
    /// </summary>
    public int Y => Rectangle.Y;

    /// <summary>
    /// The viewport width.
    /// </summary>
    public int Width => Rectangle.Width;

    /// <summary>
    /// The viewport height.
    /// </summary>
    public int Height => Rectangle.Height;

    /// <summary>
    /// The viewport rectangle.
    /// </summary>
    public readonly Rectangle Rectangle;

    /// <summary>
    /// The minimum depth.
    /// </summary>
    public readonly float MinimumDepth;

    /// <summary>
    /// The maximum depth.
    /// </summary>
    public readonly float MaximumDepth;

    public Viewport(int x, int y, int width, int height, float minDepth, float maxDepth)
    {
        Rectangle = new Rectangle(x, y, width, height);
        MinimumDepth = minDepth;
        MaximumDepth = maxDepth;
    }

    public Viewport(Rectangle rectangle, float minDepth, float maxDepth)
    {
        Rectangle = rectangle;
        MinimumDepth = minDepth;
        MaximumDepth = maxDepth;
    }

    public override bool Equals(object? obj)
    {
        return obj is Viewport other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height, Rectangle, MinimumDepth, MaximumDepth);
    }

    public bool Equals(Viewport other)
    {
        return X == other.X &&
               Y == other.Y &&
               Width == other.Width &&
               Height == other.Height &&
               Rectangle.Equals(other.Rectangle) &&
               MinimumDepth == other.MinimumDepth &&
               MaximumDepth == other.MaximumDepth;
    }

    public static bool operator ==(Viewport left, Viewport right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Viewport left, Viewport right)
    {
        return !(left == right);
    }
}
