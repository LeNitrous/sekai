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
    int Height { get; }

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
    /// The wrapping strategy used in the texture's hornizontal axis.
    /// </summary>
    WrapMode WrapModeS { get; set; }

    /// <summary>
    /// The wrapping strategy used in the texture's vertical axis.
    /// </summary>
    WrapMode WrapModeT { get; set; }

    /// <summary>
    /// Sets the data for this texture.
    /// </summary>
    void SetData(nint data, int size, int layer, int level, int offset);

    /// <summary>
    /// Gets the data from this texture.
    /// </summary>
    void GetData(nint dest, int size, int layer, int level, int offset);
}
