// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed unsafe class GLFramebuffer : Framebuffer
{
    private bool isDisposed;
    private readonly uint handle;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLFramebuffer(GL gl, FramebufferAttachment? depth, FramebufferAttachment[] colors)
    {
        GL = gl;
        handle = GL.GenFramebuffer();

        if (depth.HasValue)
        {
            ((GLTexture)depth.Value.Texture).Attach(0, depth.Value.Level, depth.Value.Layer);
        }

        Span<GLEnum> colorAttachments = stackalloc GLEnum[colors.Length];

        for (int i = 0; i < colors.Length; i++)
        {
            var color = colors[i];
            ((GLTexture)color.Texture).Attach(i, color.Level, color.Layer);
            colorAttachments[i] = GLEnum.ColorAttachment0 + i;
        }

        GL.DrawBuffers(colorAttachments);

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

    ~GLFramebuffer()
    {
        Dispose();
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
