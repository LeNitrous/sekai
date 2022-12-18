// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Mathematics;

namespace Sekai.Rendering.Primitives;

/// <summary>
/// A polygon with 4 sides.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Quad2D : IEquatable<Quad2D>
{
    /// <summary>
    /// The quad's top left vertex position.
    /// </summary>
    public Vector2 TopLeft;

    /// <summary>
    /// The quad's bottom left vertex position.
    /// </summary>
    public Vector2 BottomLeft;

    /// <summary>
    /// The quad's bottom right vertex position.
    /// </summary>
    public Vector2 BottomRight;

    /// <summary>
    /// The quad's top right vertex position.
    /// </summary>
    public Vector2 TopRight;

    public Quad2D(Vector2 topLeft, Vector2 bottomLeft, Vector2 bottomRight, Vector2 topRight)
    {
        TopLeft = topLeft;
        BottomLeft = bottomLeft;
        BottomRight = bottomRight;
        TopRight = topRight;
    }

    public override bool Equals(object? obj)
    {
        return obj is Quad2D quad && Equals(quad);
    }

    public bool Equals(Quad2D other)
    {
        return TopLeft.Equals(other.TopLeft) &&
               BottomLeft.Equals(other.BottomLeft) &&
               BottomRight.Equals(other.BottomRight) &&
               TopRight.Equals(other.TopRight);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TopLeft, BottomLeft, BottomRight, TopRight);
    }

    public static bool operator ==(Quad2D left, Quad2D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Quad2D left, Quad2D right)
    {
        return !(left == right);
    }

    public static implicit operator Quad2D(RectangleF rectangle) => new
    (
        new Vector2(rectangle.Left, rectangle.Top),
        new Vector2(rectangle.Right, rectangle.Top),
        new Vector2(rectangle.Left, rectangle.Bottom),
        new Vector2(rectangle.Right, rectangle.Bottom)
    );

    public static implicit operator Quad2D(Rectangle rectangle) => new
    (
        new Vector2(rectangle.Left, rectangle.Top),
        new Vector2(rectangle.Right, rectangle.Top),
        new Vector2(rectangle.Left, rectangle.Bottom),
        new Vector2(rectangle.Right, rectangle.Bottom)
    );
}
