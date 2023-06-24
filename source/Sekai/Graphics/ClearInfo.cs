// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Graphics;

/// <summary>
/// Describes how the framebuffer should be cleared.
/// </summary>
public struct ClearInfo : IEquatable<ClearInfo>
{
    /// <summary>
    /// The flags denoting which buffer to clear.
    /// </summary>
    public ClearFlags Flags;

    /// <summary>
    /// The color value to set when <see cref="ClearFlags.Color"/> is set.
    /// </summary>
    public Color Color;

    /// <summary>
    /// The depth value to set when <see cref="ClearFlags.Depth"/> is set.
    /// </summary>
    public float Depth;

    /// <summary>
    /// The stencil value to set when <see cref="ClearFlags.Stencil"/> is set.
    /// </summary>
    public byte Stencil;

    public ClearInfo(ClearFlags flags, Color color, float depth, byte stencil)
    {
        Flags = flags;
        Color = color;
        Depth = depth;
        Stencil = stencil;
    }

    public ClearInfo(Color color)
    {
        Flags = ClearFlags.Color;
        Color = color;
    }

    public ClearInfo(float depth)
    {
        Flags = ClearFlags.Depth;
        Depth = depth;
    }

    public ClearInfo(byte stencil)
    {
        Flags = ClearFlags.Stencil;
        Stencil = stencil;
    }

    public ClearInfo(float depth, byte stencil)
    {
        Flags = ClearFlags.Depth | ClearFlags.Stencil;
        Depth = depth;
        Stencil = stencil;
    }

    public static implicit operator ClearInfo(Color color)
    {
        return new ClearInfo(color);
    }

    public readonly bool Equals(ClearInfo other)
    {
        return Flags == other.Flags &&
               Color.Equals(other.Color) &&
               Depth == other.Depth &&
               Stencil == other.Stencil;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is ClearInfo info && Equals(info);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Flags, Color, Depth, Stencil);
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
