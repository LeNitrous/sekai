// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// A constant color that is sampled when <see cref="SamplerAddressMode.Border"/> is used.
/// </summary>
public enum SamplerBorderColor
{
    /// <summary>
    /// Transparent Black (0, 0, 0, 0)
    /// </summary>
    TransparentBlack,

    /// <summary>
    /// Opaque Black (0, 0, 0, 1)
    /// </summary>
    OpaqueBlack,

    /// <summary>
    /// Opaque White (1, 1, 1, 1)
    /// </summary>
    OpaqueWhite,
}
