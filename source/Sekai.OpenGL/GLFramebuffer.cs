// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed unsafe class GLFramebuffer : Graphics.Framebuffer
{
    private bool isDisposed;
    private readonly uint handle;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLFramebuffer(GL gl, Graphics.FramebufferAttachment? depth, Graphics.FramebufferAttachment[] colors)
    {
        GL = gl;
        handle = GL.GenFramebuffer();

        if (depth.HasValue)
        {
            ((GLTexture)depth.Value.Texture).Attach(0, depth.Value.Level, depth.Value.Layer);
        }

        if (colors is not null && colors.Length > 0)
        {
            Span<GLEnum> colorAttachments = stackalloc GLEnum[colors.Length];

            for (int i = 0; i < colors.Length; i++)
            {
                var color = colors[i];
                ((GLTexture)color.Texture).Attach(i, color.Level, color.Layer);
                colorAttachments[i] = GLEnum.ColorAttachment0 + i;
            }

            GL.DrawBuffers(colorAttachments);
        }

        if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
            throw new ArgumentException($"Failed to create framebuffer: {GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer)}");
    }

    public void Bind()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLFramebuffer));
        }

        GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        GL.DeleteFramebuffer(handle);

        isDisposed = true;
        GC.SuppressFinalize(this);
    }
}
