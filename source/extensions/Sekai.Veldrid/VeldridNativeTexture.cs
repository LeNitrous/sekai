// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridNativeTexture : VeldridBindableResource<Vd.Texture>, INativeTexture
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

    public VeldridNativeTexture(NativeTextureDescription desc, Vd.Texture resource)
        : base(resource)
    {
        Format = desc.Format;
        Width = desc.Width;
        Height = desc.Height;
        Depth = desc.Depth;
        MipLevels = desc.MipLevels;
        ArrayLayers = desc.ArrayLayers;
        Usage = desc.Usage;
        Kind = desc.Kind;
        SampleCount = desc.SampleCount;
    }
}
