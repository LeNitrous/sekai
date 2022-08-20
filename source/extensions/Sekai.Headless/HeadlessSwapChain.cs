// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessSwapChain : FrameworkObject, ISwapChain
{
    public IFramebuffer Framebuffer { get; }
    public bool VerticalSync { get; set; }

    public HeadlessSwapChain(IFramebuffer framebuffer, bool verticalSync)
    {
        Framebuffer = framebuffer;
        VerticalSync = verticalSync;
    }
}
