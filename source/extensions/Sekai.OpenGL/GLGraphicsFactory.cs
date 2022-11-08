// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Sekai.Framework.Graphics.Buffers;
using Sekai.Framework.Graphics.Shaders;
using Sekai.Framework.Graphics.Textures;

namespace Sekai.OpenGL;

internal class GLGraphicsFactory : IGraphicsFactory
{
    private readonly GLGraphicsSystem system;

    public GLGraphicsFactory(GLGraphicsSystem system)
    {
        this.system = system;
    }

    public INativeShader CreateShader(string comp) => new GLShader(system, comp);
    public INativeShader CreateShader(string vert, string frag) => new GLShader(system, vert, frag);
    public INativeBuffer CreateBuffer(int capacity, bool dynamic) => new GLBuffer(system, capacity, dynamic);

    public INativeTexture CreateTexture()
    {
        throw new System.NotImplementedException();
    }

    public INativeFrameBuffer CreateFramebuffer(INativeTexture color, RenderBufferFormat[]? depth)
    {
        throw new System.NotImplementedException();
    }
}
