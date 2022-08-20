// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface INativeTexture : IBindableResource
{
    PixelFormat Format { get; }
    uint Width { get; }
    uint Height { get; }
    uint Depth { get; }
    uint MipLevels { get; }
    uint ArrayLayers { get; }
    NativeTextureUsage Usage { get; }
    NativeTextureKind Kind { get; }
    NativeTextureSampleCount SampleCount { get; }
}
