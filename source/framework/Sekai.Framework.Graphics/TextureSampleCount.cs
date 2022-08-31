// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines the number of samples to use in a <see cref="ITexture"/>
/// </summary>
public enum TextureSampleCount
{
    /// <summary>
    /// 1 sample (no multi-sampling).
    /// </summary>
    Count1,
    /// <summary>
    /// 2 Samples.
    /// </summary>
    Count2,
    /// <summary>
    /// 4 Samples.
    /// </summary>
    Count4,
    /// <summary>
    /// 8 Samples.
    /// </summary>
    Count8,
    /// <summary>
    /// 16 Samples.
    /// </summary>
    Count16,
    /// <summary>
    /// 32 Samples.
    /// </summary>
    Count32,
}
