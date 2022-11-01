// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Describes a 3-dimensional region.
/// </summary>
public readonly struct Viewport : IEquatable<Viewport>
{
    /// <summary>
    /// The viewport X position.
    /// </summary>
    public float X => Rectangle.X;

    /// <summary>
    /// The viewport Y position.
    /// </summary>
    public float Y => Rectangle.Y;

    /// <summary>
    /// The viewport width.
    /// </summary>
    public float Width => Rectangle.Width;

    /// <summary>
    /// The viewport height.
    /// </summary>
    public float Height => Rectangle.Height;

    /// <summary>
    /// The viewport rectangle.
    /// </summary>
    public readonly RectangleF Rectangle;

    /// <summary>
    /// The minimum depth.
    /// </summary>
    public readonly float MinimumDepth;

    /// <summary>
    /// The maximum depth.
    /// </summary>
    public readonly float MaximumDepth;

    public Viewport(float x, float y, float width, float height, float minDepth, float maxDepth)
    {
        Rectangle = new RectangleF(x, y, width, height);
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
