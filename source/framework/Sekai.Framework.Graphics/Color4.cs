// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Represents a 4-channel color.
/// </summary>
public struct Color4 : IEquatable<Color4>
{
    private float r;
    private float g;
    private float b;
    private float a;

    /// <summary>
    /// The red component of the color.
    /// </summary>
    public float R
    {
        get => r;
        set => r = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// The green component of the color
    /// </summary>
    public float G
    {
        get => g;
        set => g = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// The blue component of the color.
    /// </summary>
    public float B
    {
        get => b;
        set => b = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// The alpha component of the color.
    /// </summary>
    public float A
    {
        get => a;
        set => a = Math.Clamp(value, 0f, 1f);
    }

    public Color4(float r, float g, float b, float a)
    {
        this.r = Math.Clamp(r, 0f, 1f);
        this.g = Math.Clamp(g, 0f, 1f);
        this.b = Math.Clamp(b, 0f, 1f);
        this.a = Math.Clamp(a, 0f, 1f);
    }

    public Color4(byte r, byte g, byte b, byte a)
    {
        this.r = r / 255f;
        this.g = g / 255f;
        this.b = b / 255f;
        this.a = a / 255f;
    }

    public bool Equals(Color4 other)
    {
        return r == other.r &&
               g == other.g &&
               b == other.b &&
               a == other.a;
    }

    public override bool Equals(object? obj)
    {
        return obj is Color4 other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(r, g, b, a);
    }

    public static bool operator ==(Color4 left, Color4 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Color4 left, Color4 right)
    {
        return !(left == right);
    }

    public static implicit operator Vector4(Color4 color) => new(color.R, color.G, color.B, color.A);
}
