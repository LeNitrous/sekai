// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

/// <summary>
/// Determines the color to be sampled when <see cref="TextureAddressMode.Border"/> is used.
/// </summary>
public enum TextureBorderColor
{
    /// <summary>
    /// A black transparent color.
    /// </summary>
    Transparent,

    /// <summary>
    /// A black opaque color.
    /// </summary>
    Black,

    /// <summary>
    /// A white opaque color.
    /// </summary>
    White,
}
