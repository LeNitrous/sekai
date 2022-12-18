// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;

namespace Sekai.Rendering.Batches;

public class QuadBatch2D<T> : RenderBatch<T>
    where T : unmanaged, IVertex2D, IColoredVertex, ITexturedVertex
{
    protected override int PrimitiveIndexCount => 6;
    protected override int PrimitiveVertexCount => 4;
    protected override PrimitiveTopology Topology => PrimitiveTopology.Triangles;

    public QuadBatch2D(int maxQuadCount)
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

    protected override Shader CreateShader() => new(shader);

    private static readonly string shader = @"
attrib vec2 a_Position;
attrib vec2 a_TexCoord;
attrib vec4 a_Color;
extern mat4 g_ProjMatrix;
extern sampler2D m_Texture;

void vert()
{
    SK_POSITION = g_ProjMatrix * vec4(a_Position.x, a_Position.y, 0.0, 1.0);
}

void frag()
{
    SK_COLOR0 = texture(m_Texture, a_TexCoord) * a_Color;
}
";
}
