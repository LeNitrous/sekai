// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Color represented as normalized red, blue, green, and alpha channels.
/// </summary>
public struct Color : IEquatable<Color>
{
    private float r = 1.0f;
    private float g = 1.0f;
    private float b = 1.0f;
    private float a = 1.0f;

    /// <summary>
    /// The red channel.
    /// </summary>
    public float R
    {
        get => r;
        set => r = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// The green channel.
    /// </summary>
    public float G
    {
        get => g;
        set => g = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// The blue channel.
    /// </summary>
    public float B
    {
        get => b;
        set => b = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// The alpha channel.
    /// </summary>
    public float A
    {
        get => a;
        set => a = Math.Clamp(value, 0f, 1f);
    }

    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public bool Equals(Color other)
    {
        return R == other.R
            && G == other.G
            && B == other.B
            && A == other.A;
    }

    public override bool Equals(object? obj) => obj is Color color && Equals(color);

    public override int GetHashCode() => HashCode.Combine(r, g, b, a);

    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !(left == right);
}
