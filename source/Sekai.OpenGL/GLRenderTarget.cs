// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Graphics.Textures;

namespace Sekai.OpenGL;

internal class GLRenderTarget : NativeRenderTarget
{
    private readonly uint frameBufferId;
    private readonly GLGraphicsSystem system;

    public GLRenderTarget(GLGraphicsSystem system, uint frameBufferId, IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth)
        : base(color, depth)
    {
        this.system = system;
        this.frameBufferId = frameBufferId;
    }

    protected override void Dispose(bool disposing)
    {
        system.DestroyRenderTarget(frameBufferId);
    }

    public static implicit operator uint(GLRenderTarget framebuffer) => framebuffer.frameBufferId;
}
