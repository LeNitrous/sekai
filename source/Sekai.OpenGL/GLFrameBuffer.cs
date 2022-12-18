// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Buffers;
using Sekai.Graphics.Textures;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal class GLFrameBuffer : GLResource, INativeFrameBuffer
{
    private int colorAttachmentCount;
    private readonly uint framebufferId;

    public GLFrameBuffer(GLGraphicsSystem context)
        : base(context)
    {
        framebufferId = GL.GenFramebuffer();
    }

    public void AddColorAttachment(INativeTexture texture, int level, int layer)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, this);

        var tex = (GLTexture)texture;

        if (tex.Levels > 1)
        {
            GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + colorAttachmentCount, tex, level, layer);
        }
        else
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0 + colorAttachmentCount, tex.Target, tex, level);
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

        colorAttachmentCount++;
    }

    public void SetDepthAttachment(INativeTexture texture, int level, int layer)
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, this);

        var tex = (GLTexture)texture;

        if (tex.Levels > 1)
        {
            GL.FramebufferTextureLayer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, tex, level, layer);
        }
        else
        {
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, tex.Target, tex, level);
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }

    public static implicit operator uint(GLFrameBuffer framebuffer) => framebuffer.framebufferId;
}
