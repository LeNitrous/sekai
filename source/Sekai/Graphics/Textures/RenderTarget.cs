// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Buffers;

namespace Sekai.Graphics.Textures;

public class RenderTarget : Texture, IRenderTarget
{
    private readonly Texture depth;
    private readonly FrameBuffer framebuffer;

    public RenderTarget(int width, int height, FilterMode min = FilterMode.Linear, FilterMode mag = FilterMode.Linear, WrapMode wrapModeS = WrapMode.None, WrapMode wrapModeT = WrapMode.None, TextureSampleCount sampleCount = TextureSampleCount.Count1, PixelFormat format = PixelFormat.B8_G8_R8_A8_UNorm_SRgb)
        : base(width, height, 1, 1, 1, min, mag, wrapModeS, wrapModeT, WrapMode.None, TextureType.Texture2D, TextureUsage.Sampled | TextureUsage.RenderTarget, sampleCount, format)
    {
        if (format.IsDepthStencil())
            throw new ArgumentException(@"Invalid Render Target pixel format.", nameof(format));

        depth = New2D(width, height, PixelFormat.D24_UNorm_S8_UInt, usage: TextureUsage.DepthStencil);
        framebuffer = new FrameBuffer(new FrameBufferAttachment(this, 1, 1), new FrameBufferAttachment(depth, 1, 1));
    }

    void IRenderTarget.Bind()
    {
        framebuffer.Bind();
    }

    void IRenderTarget.Unbind()
    {
        framebuffer.Unbind();
    }

    public override void Dispose()
    {
        framebuffer.Dispose();
        depth.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
