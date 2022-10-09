// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Dummy;

internal class DummySwapChain : FrameworkObject, ISwapChain
{
    public IFramebuffer Framebuffer { get; }
    public bool VerticalSync { get; set; }

    public DummySwapChain(IFramebuffer framebuffer, bool verticalSync)
    {
        Framebuffer = framebuffer;
        VerticalSync = verticalSync;
    }
}
