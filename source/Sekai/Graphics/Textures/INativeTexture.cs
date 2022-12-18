// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics.Textures;

/// <summary>
/// A texture graphics device.
/// </summary>
public interface INativeTexture : IDisposable
{
    /// <summary>
    /// The texture width.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// The texture height.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture2D"/> and <seealso cref="TextureType.Texture3D"/>.
    /// </remarks>
    int Height { get; }

    /// <summary>
    /// The texture depth.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture3D"/>.
    /// </remarks>
    int Depth { get; }

    /// <summary>
    /// The texture layer count.
    /// </summary>
    int Layers { get; }

    /// <summary>
    /// The texture mipmap levels.
    /// </summary>
    int Levels { get; }

    /// <summary>
    /// The filter strategy used when the texture is minimized.
    /// </summary>
    FilterMode Min { get; set; }

    /// <summary>
    /// The filter strategy used when the texture is maximized.
    /// </summary>
    FilterMode Mag { get; set; }

    /// <summary>
    /// The wrapping strategy used in the texture's X axis.
    /// </summary>
    WrapMode WrapModeS { get; set; }

    /// <summary>
    /// The wrapping strategy used in the texture's Y axis.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture2D"/> and <seealso cref="TextureType.Texture3D"/>.
    /// </remarks>
    WrapMode WrapModeT { get; set; }

    /// <summary>
    /// The wrapping strategy used in the textures Z axis.
    /// </summary>
    /// <remarks>
    /// Only used by <see cref="TextureType.Texture3D"/>.
    /// </remarks>
    WrapMode WrapModeR { get; set; }

    /// <summary>
    /// The pixel data format used by this texture.
    /// </summary>
    PixelFormat Format { get; }

    /// <summary>
    /// The texture type.
    /// </summary>
    TextureType Type { get; }

    /// <summary>
    /// The texture usage.
    /// </summary>
    TextureUsage Usage { get; }

    /// <summary>
    /// The texture sample count.
    /// </summary>
    TextureSampleCount SampleCount { get; }

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
    void SetData(nint data, int x, int y, int z, int width, int height, int depth, int layer, int level);
}
