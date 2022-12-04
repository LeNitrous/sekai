// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

/// <summary>
/// A line batch renderer.
/// </summary>
public abstract class LineBatch<TPrimitive, TVertex, TVector> : RenderBatch<TPrimitive, TVertex, TVector>
    where TPrimitive : unmanaged, IPrimitive<TVector>
    where TVertex : unmanaged, IVertex, IColoredVertex
    where TVector : struct, IEquatable<TVector>
{
    /// <summary>
    /// Gets the current color or sets the color after flushing the current batch.
    /// </summary>
    public Color4 Color
    {
        get => color;
        set
        {
            EnsureStarted();

            if (color.Equals(value))
                return;

            color = value;
            Flush();
        }
    }

    protected sealed override int PrimitiveIndexCount => 2;
    protected sealed override int PrimitiveVertexCount => 2;
    protected sealed override PrimitiveTopology Topology => PrimitiveTopology.Lines;

    private Color4 color;

    public LineBatch(int maxLineCount)
        : base(maxLineCount)
    {
    }

    protected sealed override ReadOnlySpan<ushort> CreateIndices(int maxIndexCount)
    {
        Span<ushort> indices = new ushort[maxIndexCount];

        for (ushort i = 0; i < maxIndexCount; i++)
            indices[i] = i;

        return indices;
    }
}
