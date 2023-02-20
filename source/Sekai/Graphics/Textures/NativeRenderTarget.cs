// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Graphics.Textures;

/// <summary>
/// A render target graphics device.
/// </summary>
public abstract class NativeRenderTarget : DisposableObject
{
    /// <summary>
    /// The render target depth buffer.
    /// </summary>
    public readonly NativeRenderBuffer? Depth;

    /// <summary>
    /// The rnder target color buffers.
    /// </summary>
    public readonly IReadOnlyList<NativeRenderBuffer> Color;

    protected NativeRenderTarget(IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth = null)
    {
        Depth = depth;
        Color = color;
    }
}
