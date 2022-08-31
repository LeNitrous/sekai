// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface ITexture : IBindableResource
{
    PixelFormat Format { get; }
    uint Width { get; }
    uint Height { get; }
    uint Depth { get; }
    uint MipLevels { get; }
    uint ArrayLayers { get; }
    TextureUsage Usage { get; }
    TextureKind Kind { get; }
    TextureSampleCount SampleCount { get; }
}
