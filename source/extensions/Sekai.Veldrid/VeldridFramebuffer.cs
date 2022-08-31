// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridFramebuffer : VeldridGraphicsResource<Vd.Framebuffer>, IFramebuffer
{
    public OutputDescription OutputDescription { get; }
    public FramebufferAttachment? DepthTarget { get; }
    public FramebufferAttachment[] ColorTargets { get; }
    public uint Width => Resource.Width;
    public uint Height => Resource.Height;

    public VeldridFramebuffer(FramebufferDescription desc, Vd.Framebuffer resource)
        : base(resource)
    {
        DepthTarget = desc.DepthTarget;
        ColorTargets = desc.ColorTargets;
        OutputDescription = new OutputDescription
        {
            DepthAttachment = resource.OutputDescription.DepthAttachment.HasValue ? toSekai(resource.OutputDescription.DepthAttachment.Value) : null,
            ColorAttachments = resource.OutputDescription.ColorAttachments.Select(toSekai).ToArray(),
        };
    }

    private static OutputAttachmentDescription toSekai(Vd.OutputAttachmentDescription desc)
    {
        return new OutputAttachmentDescription((PixelFormat)desc.Format);
    }
}
