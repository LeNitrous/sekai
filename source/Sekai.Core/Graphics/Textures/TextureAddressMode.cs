// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

/// <summary>
/// Modes of addressing texture coordintes.
/// </summary>
public enum TextureAddressMode
{
    /// <summary>
    /// Texture coordinates are wrapped upon overflow.
    /// </summary>
    Wrap,

    /// <summary>
    /// Texture coordinates that overflow are clamped to a minimum or maximum value.
    /// </summary>
    Clamp,

    /// <summary>
    /// Texture coordinates that overflow return a predefined border color.
    /// </summary>
    Border,

    /// <summary>
    /// Texture coordinates are mirrored upon overflow.
    /// </summary>
    Mirror,
}
