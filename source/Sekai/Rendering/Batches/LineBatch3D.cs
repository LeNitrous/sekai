// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

public sealed class LineBatch3D : LineBatch<Line3D, ColoredVertex3D, Vector3>
{
    public LineBatch3D(int maxLineCount)
        : base(maxLineCount)
    {
    }

    protected override Shader CreateShader() => new(shader);

    protected override ReadOnlySpan<ColoredVertex3D> CreateVertices(ReadOnlySpan<Vector3> positions)
    {
        Span<ColoredVertex3D> vertices = new ColoredVertex3D[positions.Length];

        for (int i = 0; i < positions.Length; i++)
            vertices[i] = new ColoredVertex3D { Position = positions[i], Color = Color };

        return vertices;
    }

    private static readonly string shader = @"
attrib vec3 a_Position;
attrib vec4 a_Color;
extern mat4 g_ProjMatrix;

void vert()
{
    SK_POSITION = g_ProjMatrix * vec4(a_Position.x, a_Position.y, a_Position.z, 1.0);
}

void frag()
{
    SK_COLOR0 = a_Color;
}";
}
