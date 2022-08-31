// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct TextureDescription : IEquatable<TextureDescription>
{
    /// <summary>
    /// The width of the native texture in texels.
    /// </summary>
    public uint Width;

    /// <summary>
    /// The height of the native texture in texels. Only valid if <see cref="TextureKind.Texture2D"/> is used.
    /// </summary>
    public uint Height;

    /// <summary>
    /// The height of the native texture in texels. Only valid if <see cref="TextureKind.Texture3D"/> is used.
    /// </summary>
    public uint Depth;

    /// <summary>
    /// The number of mipmap levels.
    /// </summary>
    public uint MipLevels;

    /// <summary>
    /// The number of array layers.
    /// </summary>
    public uint ArrayLayers;

    /// <summary>
    /// The format of individual texture elements.
    /// </summary>
    public PixelFormat Format;

    /// <summary>
    /// The kind of the given native texture.
    /// </summary>
    public TextureKind Kind;

    /// <summary>
    /// Determines how this native texture is used.
    /// </summary>
    public TextureUsage Usage;

    /// <summary>
    /// The number of samples of this native texture.
    /// </summary>
    public TextureSampleCount SampleCount;

    public TextureDescription(uint width, uint height, uint depth, uint mipLevels, uint arrayLayers,
                                    PixelFormat format, TextureKind kind, TextureUsage usage, TextureSampleCount sampleCount)
    {
        Width = width;
        Height = height;
        Depth = depth;
        MipLevels = mipLevels;
        ArrayLayers = arrayLayers;
        Format = format;
        Kind = kind;
        Usage = usage;
        SampleCount = sampleCount;
    }

    public override bool Equals(object? obj)
    {
        return obj is TextureDescription other && Equals(other);
    }

    public bool Equals(TextureDescription other)
    {
        return Width == other.Width &&
               Height == other.Height &&
               Depth == other.Depth &&
               MipLevels == other.MipLevels &&
               ArrayLayers == other.ArrayLayers &&
               Format == other.Format &&
               Kind == other.Kind &&
               Usage == other.Usage &&
               SampleCount == other.SampleCount;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Width);
        hash.Add(Height);
        hash.Add(Depth);
        hash.Add(MipLevels);
        hash.Add(ArrayLayers);
        hash.Add(Format);
        hash.Add(Kind);
        hash.Add(Usage);
        hash.Add(SampleCount);
        return hash.ToHashCode();
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
