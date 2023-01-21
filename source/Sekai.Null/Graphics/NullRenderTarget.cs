// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Graphics.Textures;

namespace Sekai.Null.Graphics;

internal class NullRenderTarget : NativeRenderTarget
{
    public NullRenderTarget(IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth = null)
        : base(color, depth)
    {
    }
}
