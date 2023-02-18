// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Assets;
using Sekai.Graphics;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public sealed class LineBatch2D : LineBatch<ColoredVertex2D>
{
    protected override Uri Shader { get; } = new Uri(@"./engine/shaders/batches/line2d.sksl", UriKind.Relative);

    public LineBatch2D(GraphicsContext graphics, AssetLoader assets, int maxLineCount)
        : base(graphics, assets, maxLineCount)
    {
    }
}
