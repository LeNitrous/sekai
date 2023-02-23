// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

/// <summary>
/// A texture graphics device.
/// </summary>
public abstract class NativeTexture : DisposableObject
{
    /// <summary>
    /// The texture width.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// The texture height.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture2D"/> and <seealso cref="TextureType.Texture3D"/>.
    /// </remarks>
    public int Height { get; }

    /// <summary>
    /// The texture depth.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture3D"/>.
    /// </remarks>
    public int Depth { get; }

    /// <summary>
    /// The texture layer count.
    /// </summary>
    public int Layers { get; }

    /// <summary>
    /// The texture mipmap levels.
    /// </summary>
    public int Levels { get; }

    /// <summary>
    /// The filter strategy used when the texture is minimized.
    /// </summary>
    public FilterMode Min { get; set; }

    /// <summary>
    /// The filter strategy used when the texture is maximized.
    /// </summary>
    public FilterMode Mag { get; set; }

    /// <summary>
    /// The wrapping strategy used in the texture's X axis.
    /// </summary>
    public WrapMode WrapModeS { get; set; }

    /// <summary>
    /// The wrapping strategy used in the texture's Y axis.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture2D"/> and <seealso cref="TextureType.Texture3D"/>.
    /// </remarks>
    public WrapMode WrapModeT { get; set; }

    /// <summary>
    /// The wrapping strategy used in the textures Z axis.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture3D"/>.
    /// </remarks>
    public WrapMode WrapModeR { get; set; }

    /// <summary>
    /// The pixel data format used by this texture.
    /// </summary>
    public PixelFormat Format { get; }

    /// <summary>
    /// The texture type.
    /// </summary>
    public TextureType Type { get; }

    /// <summary>
    /// The texture usage.
    /// </summary>
    public TextureUsage Usage { get; }

    /// <summary>
    /// The texture sample count.
    /// </summary>
    public TextureSampleCount SampleCount { get; }

    protected NativeTexture(int width, int height, int depth, int layers, int levels, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, PixelFormat format, TextureType type, TextureUsage usage, TextureSampleCount sampleCount)
    {
        Min = min;
        Mag = mag;
        Type = type;
        Usage = usage;
        Width = width;
        Depth = depth;
        Height = height;
        Layers = layers;
        Levels = levels;
        Format = format;
        WrapModeS = wrapModeS;
        WrapModeT = wrapModeT;
        WrapModeR = wrapModeR;
        SampleCount = sampleCount;
    }

    /// <summary>
    /// Sets the data for this texture.
    /// </summary>
    /// <param name="data">The texture data to be set.</param>
    /// <param name="x">The X offset where the data will start.</param>
    /// <param name="y">The Y offset where the data will start.</param>
    /// <param name="z">The Z offset where the data will start.</param>
    /// <param name="width">The width of the texture data.</param>
    /// <param name="height">The height of the texture data.</param>
    /// <param name="depth">The depth of the texture data.</param>
    /// <param name="layer">The array layer where data will be set.</param>
    /// <param name="level">The mip level where the data will be set.</param>
    public abstract void SetData(nint data, int x, int y, int z, int width, int height, int depth, int layer, int level);

    /// <summary>
    /// Gets the data from this texture.
    /// </summary>
    /// <param name="data">The texture data to be set.</param>
    /// <param name="level">The mip level where the data.</param>
    public abstract void GetData(nint data, int level);
}
