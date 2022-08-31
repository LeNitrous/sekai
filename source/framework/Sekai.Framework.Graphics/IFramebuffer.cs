// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface IFramebuffer : IGraphicsResource
{
    OutputDescription OutputDescription { get; }
    FramebufferAttachment? DepthTarget { get; }
    FramebufferAttachment[] ColorTargets { get; }
    uint Width { get; }
    uint Height { get; }
}
