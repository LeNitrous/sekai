// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Graphics.Textures;

/// <summary>
/// A render target graphics device.
/// </summary>
public abstract class NativeRenderTarget : FrameworkObject
{
    public readonly NativeRenderBuffer? Depth;
    public readonly IReadOnlyList<NativeRenderBuffer> Color;

    protected NativeRenderTarget(IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth = null)
    {
        Depth = depth;
        Color = color;
    }
}
