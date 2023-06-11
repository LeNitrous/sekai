// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of operations that occur during stencil testing.
/// </summary>
public enum StencilOperation
{
    /// <summary>
    /// Keep the existing stencil data.
    /// </summary>
    Keep,

    /// <summary>
    /// Set the stencil data to zero.
    /// </summary>
    Zero,

    /// <summary>
    /// Replace the existing stencil data with the reference value.
    /// </summary>
    Replace,

    /// <summary>
    /// Increment the stencil value by one and clamp the result.
    /// </summary>
    Increment,

    /// <summary>
    /// Increment the stencil value by one and wrap the result.
    /// </summary>
    IncrementWrap,

    /// <summary>
    /// Decrement the stencil value by one and clamp the result.
    /// </summary>
    Decrement,

    /// <summary>
    /// Decrement the stencil value by one and wrap the result.
    /// </summary>
    DecrementWrap,

    /// <summary>
    /// Invert the stencil data.
    /// </summary>
    Invert,
}
