// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// The flags used to clear the target framebuffer.
/// </summary>
[Flags]
public enum ClearFlags
{
    /// <summary>
    /// Clear nothing.
    /// </summary>
    None,

    /// <summary>
    /// Clear the color buffer.
    /// </summary>
    Color = 1 << 0,

    /// <summary>
    /// Clear the depth buffer.
    /// </summary>
    Depth = 1 << 1,

    /// <summary>
    /// Clear the stencil buffer.
    /// </summary>
    Stencil = 1 << 2,
}
