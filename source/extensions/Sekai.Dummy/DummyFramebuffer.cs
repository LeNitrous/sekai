// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Dummy;

internal class DummyFramebuffer : FrameworkObject, IFramebuffer
{
    public OutputDescription OutputDescription { get; }
    public FramebufferAttachment? DepthTarget { get; }
    public FramebufferAttachment[] ColorTargets { get; }
    public uint Width { get; }
    public uint Height { get; }

    public DummyFramebuffer(FramebufferDescription desc)
    {
        Width = desc.ColorTargets.FirstOrDefault().Target.Width;
        Height = desc.ColorTargets.FirstOrDefault().Target.Height;
        DepthTarget = desc.DepthTarget;
        ColorTargets = desc.ColorTargets;
    }
}
