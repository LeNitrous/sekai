// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Graphics.Textures;

namespace Sekai.Graphics.Buffers;

/// <summary>
/// Contains data necessary for blitting an image onto the screen.
/// </summary>
public sealed class FrameBuffer : GraphicsObject
{
    /// <summary>
    /// The depth attachment of this framebuffer.
    /// </summary>
    public readonly FrameBufferAttachment? Depth;

    /// <summary>
    /// THe color attachments of this framebuffer.
    /// </summary>
    public readonly IReadOnlyList<FrameBufferAttachment> Color;

    internal readonly INativeFrameBuffer Native;

    public FrameBuffer(FrameBufferAttachment color, FrameBufferAttachment? depth = null)
        : this(new[] { color }, depth)
    {
    }

    public FrameBuffer(FrameBufferAttachment[] color, FrameBufferAttachment? depth = null)
    {
        Native = Context.Factory.CreateFramebuffer();

        if (depth.HasValue)
        {
            if (!depth.Value.Target.Format.IsDepthStencil() || depth.Value.Target.Type != TextureType.Texture2D)
                throw new ArgumentException(@"Depth attachment is not a valid depth target.", nameof(depth));

            Native.SetDepthAttachment(depth.Value.Target.Native, depth.Value.Level, depth.Value.Layer);
        }

        foreach (var attach in color)
        {
            if (attach.Target.Format.IsDepthStencil() || attach.Target.Type != TextureType.Texture2D)
                throw new ArgumentException(@"Color attachment is not a valid color target.", nameof(color));

            Native.SetDepthAttachment(attach.Target.Native, attach.Level, attach.Layer);
        }

        Color = color;
        Depth = depth;
    }

    /// <summary>
    /// Makes this framebuffer the current.
    /// </summary>
    public void Bind()
    {
        Context.BindFrameBuffer(this);
    }

    /// <summary>
    /// Makes this framebuffer not the current.
    /// </summary>
    public void Unbind()
    {
        Context.UnbindFrameBuffer(this);
    }

    protected override void Destroy() => Native.Dispose();
}
