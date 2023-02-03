// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public sealed class QuadBatch : RenderBatch<TexturedVertex2D>
{
    protected override Uri Shader { get; } = new Uri(@"./engine/shaders/batches/quad.sksl", UriKind.Relative);
    protected override int PrimitiveIndexCount => 6;
    protected override int PrimitiveVertexCount => 4;
    protected override PrimitiveTopology Topology => PrimitiveTopology.Triangles;

    public QuadBatch(int maxQuadCount)
        : base(maxQuadCount)
    {
    }

    protected override void CreateIndices(Span<ushort> buffer)
    {
        for (int i = 0, j = 0; j < buffer.Length; i += 4, j += 6)
        {
            buffer[j + 0] = (ushort)(i + 0);
            buffer[j + 1] = (ushort)(i + 1);
            buffer[j + 2] = (ushort)(i + 3);
            buffer[j + 3] = (ushort)(i + 2);
            buffer[j + 4] = (ushort)(i + 3);
            buffer[j + 5] = (ushort)(i + 0);
        }
    }
}
