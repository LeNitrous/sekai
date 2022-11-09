// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Rendering.Vertices;

[StructLayout(LayoutKind.Sequential)]
public struct TexturedVertex2D : IVertex2D, IEquatable<TexturedVertex2D>
{
    /// <summary>
    /// The vertex position.
    /// </summary>
    public Vector2 Position;

    /// <summary>
    /// The vertex texture coordinate.
    /// </summary>
    public Vector2 TexCoord;

    public override bool Equals(object? obj)
    {
        return obj is TexturedVertex2D d && Equals(d);
    }

    public bool Equals(TexturedVertex2D other)
    {
        return Position.Equals(other.Position) &&
               TexCoord.Equals(other.TexCoord);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, TexCoord);
    }

    public static bool operator ==(TexturedVertex2D left, TexturedVertex2D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TexturedVertex2D left, TexturedVertex2D right)
    {
        return !(left == right);
    }

    Vector2 IVertex2D.Position
    {
        get => Position;
        set => Position = value;
    }
}
