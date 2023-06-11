// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of blending operations.
/// </summary>
public enum BlendOperation
{
    /// <summary>
    /// Adds the source and destination colors.
    /// </summary>
    Add,

    /// <summary>
    /// Subtracts the destination color from the source color.
    /// </summary>
    Subtract,

    /// <summary>
    /// Subtracts the source color from the destination color.
    /// </summary>
    ReverseSubtract,

    /// <summary>
    /// The minimum of each component of the source and destination colors.
    /// </summary>
    Minimum,

    /// <summary>
    /// The minimum of each component of the destination and source colors.
    /// </summary>
    Maximum,
}
