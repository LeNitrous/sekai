// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessFramebuffer : FrameworkObject, IFramebuffer
{
    public FramebufferAttachment? DepthTarget { get; }
    public FramebufferAttachment[] ColorTargets { get; }

    public HeadlessFramebuffer(FramebufferDescription desc)
    {
        DepthTarget = desc.DepthTarget;
        ColorTargets = desc.ColorTargets;
    }
}
