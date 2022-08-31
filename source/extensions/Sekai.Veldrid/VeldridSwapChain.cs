// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Linq;
using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridSwapChain : VeldridGraphicsResource<Vd.Swapchain>, ISwapChain
{
    public IFramebuffer Framebuffer { get; }

    public bool VerticalSync
    {
        get => Resource.SyncToVerticalBlank;
        set => Resource.SyncToVerticalBlank = value;
    }

    public VeldridSwapChain(Vd.Swapchain resource)
        : base(resource)
    {
        Framebuffer = new VeldridFramebuffer
        (
            new FramebufferDescription
            (
                resource.Framebuffer.DepthTarget.HasValue ? fromVeldrid(resource.Framebuffer.DepthTarget.Value) : null,
                resource.Framebuffer.ColorTargets.Select(fromVeldrid).ToArray()
            ),
            resource.Framebuffer
        );
    }

    private static FramebufferAttachment fromVeldrid(Vd.FramebufferAttachment attach)
    {
        return new FramebufferAttachment
        (
            new VeldridTexture
            (
                new TextureDescription
                (
                    attach.Target.Width,
                    attach.Target.Height,
                    attach.Target.Depth,
                    attach.Target.MipLevels,
                    attach.Target.ArrayLayers,
                    (PixelFormat)attach.Target.Format,
                    (TextureKind)attach.Target.Type,
                    (TextureUsage)attach.Target.Usage,
                    (TextureSampleCount)attach.Target.SampleCount
                ),
                attach.Target
            ),
            attach.ArrayLayer,
            attach.MipLevel
        );
    }
}
