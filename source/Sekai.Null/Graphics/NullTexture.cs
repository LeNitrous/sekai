// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Textures;

namespace Sekai.Null.Graphics;

internal class NullTexture : NativeTexture
{
    public NullTexture(int width, int height, int depth, int layers, int levels, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, PixelFormat format, TextureType type, TextureUsage usage, TextureSampleCount sampleCount)
        : base(width, height, depth, layers, levels, min, mag, wrapModeS, wrapModeT, wrapModeR, format, type, usage, sampleCount)
    {
    }

    public override void GetData(nint data, int level)
    {
    }

    public override void SetData(nint data, int x, int y, int z, int width, int height, int depth, int layer, int level)
    {
    }
}
