// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public sealed class LineBatch3D : LineBatch<ColoredVertex3D>
{
    protected override string Shader => @"engine/shaders/batches/line3d.sksl";

    public LineBatch3D(int maxLineCount)
        : base(maxLineCount)
    {
    }
}
