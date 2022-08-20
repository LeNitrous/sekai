// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface ISwapChain : IGraphicsResource
{
    /// <summary>
    /// The framebuffer used by this swapchain.
    /// </summary>
    IFramebuffer Framebuffer { get; }

    /// <summary>
    /// Whether to sync the presentation to the window system's vertical refresh rate.
    /// </summary>
    bool VerticalSync { get; set; }
}
