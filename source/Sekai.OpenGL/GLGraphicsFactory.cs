// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;

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

    public INativeTexture CreateTexture(int width, int height, int depth, int level, int layers, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, TextureType type, TextureUsage usage, TextureSampleCount sampleCount, PixelFormat format)
    {
        return new GLTexture(system, width, height, depth, layers, level, min, mag, wrapModeS, wrapModeT, wrapModeR, format, type, usage, sampleCount);
    }

    public INativeFrameBuffer CreateFramebuffer(INativeTexture color, RenderBufferFormat[]? depth)
    {
        throw new System.NotImplementedException();
    }
}
