// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics.Textures;

/// <summary>
/// Determines how a values are sampled from a texture.
/// </summary>
[Flags]
public enum TextureFilteringMode
{
    /// <summary>
    /// Point filtering for minification.
    /// </summary>
    MinPoint = 0,

    /// <summary>
    /// Point filtering for magnification.
    /// </summary>
    MagPoint = 1 << 1,

    /// <summary>
    /// Point filtering for mip-level sampling.
    /// </summary>
    MipPoint = 1 << 2,

    /// <summary>
    /// Linear filtering for minification.
    /// </summary>
    MinLinear = 1 << 4,

    /// <summary>
    /// Linear filtering for magnification.
    /// </summary>
    MagLinear = 1 << 6,

    /// <summary>
    /// Linear filtering for mip-level sampling.
    /// </summary>
    MipLinear = 1 << 8,

    /// <summary>
    /// Anisotropic filering.
    /// </summary>
    /// <remarks>
    /// This flag cannot be used with any other flags.
    /// </remarks>
    Anisotropic = int.MaxValue,
}
