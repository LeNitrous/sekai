// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

public sealed class LineBatch2D : LineBatch<Line2D, ColoredVertex2D, Vector2>, IRenderBatch2D<Line2D>
{
    public LineBatch2D(int maxLineCount)
        : base(maxLineCount)
    {
    }

    protected override Shader CreateShader() => new(shader);

    protected override void CreateVertices(Span<ColoredVertex2D> vertices, ReadOnlySpan<Vector2> positions)
    {
        for (int i = 0; i < positions.Length; i++)
            vertices[i] = new ColoredVertex2D { Position = positions[i], Color = Color };
    }

    private static readonly string shader = @"
attrib vec2 a_Position;
attrib vec4 a_Color;
extern mat4 g_ProjMatrix;

void vert()
{
    SK_POSITION = g_ProjMatrix * vec4(a_Position.x, a_Position.y, 0.0, 1.0);
}

void frag()
{
    SK_COLOR0 = a_Color;
}";
}
