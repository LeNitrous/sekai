// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public sealed class LineBatch2D : LineBatch<ColoredVertex2D>
{
    protected override string Shader => @"engine/shaders/batches/line2d.sksl";

    public LineBatch2D(int maxLineCount)
        : base(maxLineCount)
    {
    }
}
