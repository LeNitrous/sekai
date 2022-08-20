// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface IFramebuffer : IGraphicsResource
{
    FramebufferAttachment? DepthTarget { get; }
    FramebufferAttachment[] ColorTargets { get; }
}
