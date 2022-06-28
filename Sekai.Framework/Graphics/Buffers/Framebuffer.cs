// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace Sekai.Framework.Graphics.Buffers;

public class Framebuffer : GraphicsObject<Veldrid.Framebuffer>
{
    /// <summary>
    /// The main swap chain frame buffer. May be null if the graphics context does not have a swap chain.
    /// </summary>
    public static Framebuffer Default => default_lazy.Value;

    /// <summary>
    /// The depth target attached to this frame buffer. May be null if unused.
    /// </summary>
    public readonly FramebufferAttachment? DepthTarget;

    /// <summary>
    /// The color targets attached to this frame buffer.
    /// </summary>
    public readonly IReadOnlyList<FramebufferAttachment> ColorTargets;

    /// <summary>
    /// The output information for this frame buffer.
    /// </summary>
    public readonly OutputInfo Output;

    internal override Veldrid.Framebuffer Resource { get; }

    public Framebuffer(params FramebufferAttachment[] attachments)
    {
        DepthTarget = attachments.SingleOrDefault(a => a.Kind == FramebufferAttachmentKind.DepthTarget);
        ColorTargets = attachments.Where(a => a.Kind == FramebufferAttachmentKind.ColorTarget).ToArray();

        if (DepthTarget.HasValue && !DepthTarget.Value.Target.Usage.HasFlag(Textures.TextureUsage.DepthStencil))
            throw new ArgumentException($"Depth target must have {nameof(Textures.TextureUsage.DepthStencil)}");

        if (ColorTargets.Count <= 0)
            throw new ArgumentException(@"A framebuffer must at least have one color target.");

        for (int i = 0; i < ColorTargets.Count; i++)
        {
            var target = ColorTargets[i];

            if (!target.Target.Usage.HasFlag(Textures.TextureUsage.RenderTarget))
                throw new ArgumentException($"Color target must have {nameof(Textures.TextureUsage.RenderTarget)}");
        }

        FramebufferDescription description = !DepthTarget.HasValue
            ? new(null, ColorTargets.Select(t => new FramebufferAttachmentDescription(t.Target.Resource, (uint)t.Layer, (uint)t.MipLevel)).ToArray())
            : new
            (
                new FramebufferAttachmentDescription(DepthTarget.Value.Target.Resource, (uint)DepthTarget.Value.Layer, (uint)DepthTarget.Value.MipLevel),
                ColorTargets.Select(t => new FramebufferAttachmentDescription(t.Target.Resource, (uint)t.Layer, (uint)t.MipLevel)).ToArray()
            );

        Resource = Context.Resources.CreateFramebuffer(description);
        Output = new OutputInfo
        (
            Textures.TextureSampleCount.Count1,
            DepthTarget.HasValue ? new OutputAttachmentInfo(DepthTarget.Value.Target.Format) : null,
            ColorTargets.Select(a => new OutputAttachmentInfo(a.Target.Format)).ToArray()
        );
    }

    internal Framebuffer(Veldrid.Framebuffer framebuffer)
    {
        Resource = framebuffer;
        ColorTargets = framebuffer.ColorTargets.Select(c => new FramebufferAttachment(new Textures.Texture(c.Target), FramebufferAttachmentKind.ColorTarget, (int)c.ArrayLayer, (int)c.MipLevel)).ToArray();

        if (framebuffer.DepthTarget.HasValue)
            DepthTarget = new FramebufferAttachment(new Textures.Texture(framebuffer.DepthTarget.Value.Target), FramebufferAttachmentKind.DepthTarget, (int)framebuffer.DepthTarget.Value.ArrayLayer, (int)framebuffer.DepthTarget.Value.MipLevel);

        Output = new OutputInfo
        (
            Textures.TextureSampleCount.Count1,
            DepthTarget.HasValue ? new OutputAttachmentInfo(DepthTarget.Value.Target.Format) : null,
            ColorTargets.Select(a => new OutputAttachmentInfo(a.Target.Format)).ToArray()
        );
    }

    private static readonly Lazy<Framebuffer> default_lazy = new(create_swapchain_framebuffer);

    private static Framebuffer create_swapchain_framebuffer()
    {
        var context = (GraphicsContext)Game.Current.Services.Resolve<IGraphicsContext>(true);

        if (context.Device.SwapchainFramebuffer == null)
            return null;

        return new Framebuffer(context.Device.SwapchainFramebuffer);
    }
}
