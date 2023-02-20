// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Assets;
using Sekai.Graphics;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public abstract class LineBatch<T> : RenderBatch<T>
    where T : unmanaged, IVertex, IColoredVertex
{
    protected override int PrimitiveIndexCount => 2;
    protected override int PrimitiveVertexCount => 2;
    protected override PrimitiveTopology Topology => PrimitiveTopology.Lines;

    protected LineBatch(GraphicsContext graphics, AssetLoader assets, int maxLineCount)
        : base(graphics, assets, maxLineCount)
    {
    }

    protected override void CreateIndices(Span<ushort> buffer)
    {
        for (ushort i = 0; i < buffer.Length; i++)
            buffer[i] = i;
    }
}
