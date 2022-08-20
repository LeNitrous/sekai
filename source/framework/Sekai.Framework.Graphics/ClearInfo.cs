// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Information on how the current frame buffer should be cleared.
/// </summary>
public readonly struct ClearInfo : IEquatable<ClearInfo>
{
    /// <summary>
    /// The color to write to the frame buffer.
    /// </summary>
    public readonly Color4 Color;

    /// <summary>
    /// The depth to write to the frame buffer.
    /// </summary>
    public readonly double Depth;

    /// <summary>
    /// The stencil value to write to the frame buffer.
    /// </summary>
    public readonly int Stencil;

    public ClearInfo(Color4 color = default, double depth = 1f, int stencil = 0)
    {
        Color = color;
        Depth = depth;
        Stencil = stencil;
    }

    public bool Equals(ClearInfo other)
    {
        return Color == other.Color && Depth == other.Depth && Stencil == other.Stencil;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Color, Depth, Stencil);
    }

    public override bool Equals(object? obj)
    {
        return obj is ClearInfo info && Equals(info);
    }

    public static bool operator ==(ClearInfo left, ClearInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(ClearInfo left, ClearInfo right)
    {
        return !(left == right);
    }
}
