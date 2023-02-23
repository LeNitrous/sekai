// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Textures;
using Silk.NET.OpenGL;
using SekaiPixelFmt = Sekai.Graphics.Textures.PixelFormat;

namespace Sekai.OpenGL;

internal unsafe class GLTexture : NativeTexture
{
    public readonly TextureTarget Target;

    private readonly uint textureId;
    private readonly GLGraphicsSystem system;

    public GLTexture(GLGraphicsSystem system, uint textureId, int width, int height, int depth, int layers, int levels, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, SekaiPixelFmt format, TextureType type, TextureUsage usage, TextureSampleCount sampleCount, TextureTarget target)
        : base(width, height, depth, layers, levels, min, mag, wrapModeS, wrapModeT, wrapModeR, format, type, usage, sampleCount)
    {
        Target = target;
        this.system = system;
        this.textureId = textureId;
    }

    public override void GetData(nint data, int level)
        => system.GetTextureData(textureId, Target, Format, data, level);

    public override void SetData(nint data, int x, int y, int z, int width, int height, int depth, int layer, int level)
        => system.SetTextureData(textureId, Width, Height, Depth, Target, Format, data, x, y, z, width, height, depth, layer, level);

    protected override void Dispose(bool disposing)
    {
        system.DestroyTexture(textureId);
    }

    public static implicit operator uint(GLTexture texture) => texture.textureId;
}
