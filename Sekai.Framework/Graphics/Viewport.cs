// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Framework.Graphics;

public struct Viewport : IEquatable<Viewport>
{
    public float X => Rectangle.Left;
    public float Y => Rectangle.Top;
    public float Width => Rectangle.Width;
    public float Height => Rectangle.Height;
    public readonly RectangleF Rectangle;
    public readonly float MinimumDepth;
    public readonly float MaximumDepth;

    public Viewport(RectangleF rectangle, float minimumDepth, float maximumDepth)
    {
        Rectangle = rectangle;
        MinimumDepth = minimumDepth;
        MaximumDepth = maximumDepth;
    }

    public bool Equals(Viewport other)
    {
        return Rectangle == other.Rectangle
            && MinimumDepth == other.MinimumDepth
            && MaximumDepth == other.MaximumDepth;
    }

    public override bool Equals(object? obj) => obj is Viewport viewport && Equals(viewport);

    public override int GetHashCode() => HashCode.Combine(Rectangle, MinimumDepth, MaximumDepth);

    public static bool operator ==(Viewport left, Viewport right) => left.Equals(right);

    public static bool operator !=(Viewport left, Viewport right) => !(left == right);
}
