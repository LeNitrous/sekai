// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

/// <summary>
/// Determines the operation performed on samples that pass or fail the stencil test.
/// </summary>
public enum StencilOperation
{
    /// <summary>
    /// Keeps the existing value.
    /// </summary>
    Keep,

    /// <summary>
    /// Sets the value to 0.
    /// </summary>
    Zero,

    /// <summary>
    /// Replaces the existing value with <see cref="StencilInfo.Reference"/>.
    /// </summary>
    Replace,

    /// <summary>
    /// Increments the existing value and clamps it to the <see cref="uint.MaxValue"/>.
    /// </summary>
    IncrementAndClamp,

    /// <summary>
    /// Decrements the existing value and clamps it to 0.
    /// </summary>
    DecrementAndClamp,

    /// <summary>
    /// Bitwise inverts the existing value.
    /// </summary>
    Invert,

    /// <summary>
    /// Increments the existing value and wraps around back to 0 when it exceeds <see cref="uint.MaxValue"/>.
    /// </summary>
    IncrementAndWrap,

    /// <summary>
    /// Decrements the existing value and wraps around back to <see cref="uint.MaxValue"/> if the value decreases below 0.
    /// </summary>
    DecrementAndWrap,
}
