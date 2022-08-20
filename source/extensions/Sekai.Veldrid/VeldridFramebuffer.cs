// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridFramebuffer : VeldridGraphicsResource<Vd.Framebuffer>, IFramebuffer
{
    public FramebufferAttachment? DepthTarget { get; }
    public FramebufferAttachment[] ColorTargets { get; }

    public VeldridFramebuffer(FramebufferDescription desc, Vd.Framebuffer resource)
        : base(resource)
    {
        DepthTarget = desc.DepthTarget;
        ColorTargets = desc.ColorTargets;
    }
}
