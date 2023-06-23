// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Describes a <see cref="Texture"/>.
/// </summary>
public struct TextureDescription : IEquatable<TextureDescription>
{
    /// <summary>
    /// The type of this texture.
    /// </summary>
    public TextureType Type;

    /// <summary>
    /// The width of the texture.
    /// </summary>
    public int Width;

    /// <summary>
    /// The height of the texture.
    /// </summary>
    public int Height;

    /// <summary>
    /// The depth of the texture.
    /// </summary>
    public int Depth;

    /// <summary>
    /// The format of this texture.
    /// </summary>
    public PixelFormat Format;

    /// <summary>
    /// The number of mipmaps this texture will hold.
    /// </summary>
    public int Levels;

    /// <summary>
    /// The size of the texture array in elements.
    /// </summary>
    public int Layers;

    /// <summary>
    /// The usage of this texture.
    /// </summary>
    public TextureUsage Usage;

    /// <summary>
    /// The sample count of this texture.
    /// </summary>
    public TextureSampleCount Count;

    public TextureDescription(TextureType type, int width, int height, int depth, PixelFormat format, int levels, int layers, TextureUsage usage, TextureSampleCount count = TextureSampleCount.Count1)
    {
        Type = type;
        Width = width;
        Height = height;
        Depth = depth;
        Format = format;
        Levels = levels;
        Layers = layers;
        Usage = usage;
        Count = count;
    }

    public TextureDescription(int width, PixelFormat format, int levels, int layers, TextureUsage usage, TextureSampleCount count = TextureSampleCount.Count1)
        : this(TextureType.Texture1D, width, 0, 0, format, levels, layers, usage, count)
    {
    }

    public TextureDescription(int width, int height, PixelFormat format, int levels, int layers, TextureUsage usage, TextureSampleCount count = TextureSampleCount.Count1)
        : this(TextureType.Texture2D, width, height, 0, format, levels, layers, usage, count)
    {
    }

    public TextureDescription(int width, int height, int depth, PixelFormat format, int levels, int layers, TextureUsage usage, TextureSampleCount count = TextureSampleCount.Count1)
        : this(TextureType.Texture3D, width, height, depth, format, levels, layers, usage, count)
    {
    }

    public readonly bool Equals(TextureDescription other)
    {
        return Type == other.Type &&
               Width == other.Width &&
               Height == other.Height &&
               Depth == other.Depth &&
               Format == other.Format &&
               Levels == other.Levels &&
               Layers == other.Layers &&
               Usage == other.Usage &&
               Count == other.Count;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is TextureDescription other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Type);
        hashCode.Add(Width);
        hashCode.Add(Height);
        hashCode.Add(Depth);
        hashCode.Add(Format);
        hashCode.Add(Levels);
        hashCode.Add(Layers);
        hashCode.Add(Usage);
        hashCode.Add(Count);
        return hashCode.ToHashCode();
    }

    public static bool operator ==(TextureDescription left, TextureDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TextureDescription left, TextureDescription right)
    {
        return !(left == right);
    }
}
