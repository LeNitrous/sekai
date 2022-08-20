// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Headless;

internal class HeadlessNativeTexture : FrameworkObject, INativeTexture
{
    public PixelFormat Format { get; }
    public uint Width { get; }
    public uint Height { get; }
    public uint Depth { get; }
    public uint MipLevels { get; }
    public uint ArrayLayers { get; }
    public NativeTextureUsage Usage { get; }
    public NativeTextureKind Kind { get; }
    public NativeTextureSampleCount SampleCount { get; }

    public HeadlessNativeTexture(PixelFormat format, uint width, uint height, uint depth, uint mipLevels, uint arrayLayers, NativeTextureUsage usage, NativeTextureKind kind, NativeTextureSampleCount sampleCount)
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
