// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines how a values are sampled from a texture.
/// </summary>
public enum TextureFilter
{
    /// <summary>
    /// Use anisotropic filtering.
    /// </summary>
    Anisotropic,

    /// <summary>
    /// Use point filtering for min, mag, and mip filters.
    /// </summary>
    MinMagMipPoint,

    /// <summary>
    /// Use point filtering for min and mag filters and linear filtering for mip filters.
    /// </summary>
    MinMagPointMipLinear,

    /// <summary>
    /// Use point filtering for min and mip filters and linear filtering for mag filters.
    /// </summary>
    MinPointMagLinearMipPoint,

    /// <summary>
    /// Use point filtering for min filters and linear filtering for mag and mip filters.
    /// </summary>
    MinPointMagMipLinear,

    /// <summary>
    /// Use linear filtering for min filters and point filtering for mag and pin filters.
    /// </summary>
    MinLinearMagMipPoint,

    /// <summary>
    /// Use linear filtering for min and mip filters and point filtering for mag filters.
    /// </summary>
    MinLinearMagPointMipLinear,

    /// <summary>
    /// Use linear filtering for min and mag filters and point filtering for mip filters.
    /// </summary>
    MinMagLinearMipPoint,

    /// <summary>
    /// Use linear filtering for min, mag, and mip filters.
    /// </summary>
    MinMagMipLinear,
}
