// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics;

/// <summary>
/// Determines the operation used during stencil tests.
/// </summary>
public enum StencilOperation
{
    /// <summary>
    /// Set the stencil buffer to zero.
    /// </summary>
    Zero,

    /// <summary>
    /// Bitwise invert the stencil buffer.
    /// </summary>
    Invert,

    /// <summary>
    /// Do not change the stencil buffer.
    /// </summary>
    Keep,

    /// <summary>
    /// Replace the stencil buffer with the.
    /// </summary>
    Replace,

    /// <summary>
    /// Increment the stencil buffer by 1 if it's lower than the maximum value.
    /// </summary>
    Increment,

    /// <summary>
    /// Decrement the stencil buffer by 1 if it's higher than 0.
    /// </summary>
    Decrement,

    /// <summary>
    /// Increase the stencil buffer by 1 and wrap to 0 if the result is above the maximum value.
    /// </summary>
    IncrementWrapped,

    /// <summary>
    /// Decrement the stencil buffer by 1 and wrap to maximum value if the result is below 0.
    /// </summary>
    DecrementWrapped,
}
