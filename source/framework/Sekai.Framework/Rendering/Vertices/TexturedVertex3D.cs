// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Framework.Rendering.Vertices;

[StructLayout(LayoutKind.Sequential)]
public struct TexturedVertex3D : IVertex3D, IEquatable<TexturedVertex3D>
{
    /// <summary>
    /// The vertex position.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The vertex texture coordinate.
    /// </summary>
    public Vector2 TexCoord;

    public override bool Equals(object? obj)
    {
        return obj is TexturedVertex3D d && Equals(d);
    }

    public bool Equals(TexturedVertex3D other)
    {
        return Position.Equals(other.Position) && TexCoord.Equals(other.TexCoord);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Position, TexCoord);
    }

    public static bool operator ==(TexturedVertex3D left, TexturedVertex3D right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TexturedVertex3D left, TexturedVertex3D right)
    {
        return !(left == right);
    }

    Vector3 IVertex3D.Position
    {
        get => Position;
        set => Position = value;
    }
}
