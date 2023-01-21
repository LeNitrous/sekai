// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Graphics;

public readonly struct ScissorInfo : IEquatable<ScissorInfo>
{
    /// <summary>
    /// Whether scissor is enabled.
    /// </summary>
    public readonly bool Enabled;

    /// <summary>
    /// The scissor rectangle.
    /// </summary>
    public readonly Rectangle Rectangle;

    public ScissorInfo(bool enabled, Rectangle rectangle = default)
    {
        Enabled = enabled;
        Rectangle = rectangle;
    }

    public override bool Equals(object? obj)
    {
        return obj is ScissorInfo info && Equals(info);
    }

    public bool Equals(ScissorInfo other)
    {
        return Enabled == other.Enabled && Rectangle.Equals(other.Rectangle);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Enabled, Rectangle);
    }

    public static bool operator ==(ScissorInfo left, ScissorInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ScissorInfo left, ScissorInfo right)
    {
        return !(left == right);
    }
}
