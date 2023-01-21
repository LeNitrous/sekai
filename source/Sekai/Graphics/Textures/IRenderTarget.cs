// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

public interface IRenderTarget
{
    /// <summary>
    /// The render target's width.
    /// </summary>
    int Width { get; }

    /// <summary>
    /// The render target's height.
    /// </summary>
    int Height { get; }

    internal NativeRenderTarget Native { get; }
}
