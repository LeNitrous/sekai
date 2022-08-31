// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessTexture : FrameworkObject, ITexture
{
    public PixelFormat Format { get; }
    public uint Width { get; }
    public uint Height { get; }
    public uint Depth { get; }
    public uint MipLevels { get; }
    public uint ArrayLayers { get; }
    public TextureUsage Usage { get; }
    public TextureKind Kind { get; }
    public TextureSampleCount SampleCount { get; }

    public HeadlessTexture(PixelFormat format, uint width, uint height, uint depth, uint mipLevels, uint arrayLayers, TextureUsage usage, TextureKind kind, TextureSampleCount sampleCount)
    {
        Format = format;
        Width = width;
        Height = height;
        Depth = depth;
        MipLevels = mipLevels;
        ArrayLayers = arrayLayers;
        Usage = usage;
        Kind = kind;
        SampleCount = sampleCount;
    }
}
