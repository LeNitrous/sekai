// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// An addressing mode for texture coordinates.
/// </summary>
public enum SamplerAddressMode
{
    /// <summary>
    /// Texture coordinates are wrapped upon overflow.
    /// </summary>
    Wrap,

    /// <summary>
    /// Texture coordinates are mirrored upon overflow.
    /// </summary>
    Mirror,

    /// <summary>
    /// Texture coordinates are clamped to the maximum or minimum values upon overflow.
    /// </summary>
    Clamp,

    /// <summary>
    /// Texture coordinates that overflow return the predefined border color defined in
    /// <see cref="SamplerDescription.BorderColor"/>.
    /// </summary>
    Border,

}