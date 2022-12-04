// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai.Graphics;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering.Batches;

public class QuadBatch : RenderBatch<Quad, TexturedVertex2D, Vector2>, IRenderBatch2D<Quad>
{
    public Texture Texture
    {
        get => texture;
        set
        {
            EnsureStarted();

            if (texture == value)
                return;

            texture = value;
            Flush();
        }
    }

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

    protected override int PrimitiveIndexCount => 6;
    protected override int PrimitiveVertexCount => 4;
    protected override PrimitiveTopology Topology => PrimitiveTopology.Triangles;

    private Color4 color = Color4.White;
    private Texture texture;

    public QuadBatch(int maxQuadCount)
        : base(maxQuadCount)
    {
        texture = Context.WhitePixel;
    }

    protected override Shader CreateShader() => new(shader);

    protected override void OnBegin()
    {
        texture.Bind();
    }

    protected override void OnEnd()
    {
        texture.Unbind();
    }

    protected override ReadOnlySpan<ushort> CreateIndices(int maxIndexCount)
    {
        Span<ushort> indices = new ushort[maxIndexCount];

        for (int i = 0, j = 0; j < maxIndexCount; i += 4, j += 6)
        {
            indices[j + 0] = (ushort)(i + 0);
            indices[j + 1] = (ushort)(i + 1);
            indices[j + 2] = (ushort)(i + 3);
            indices[j + 3] = (ushort)(i + 2);
            indices[j + 4] = (ushort)(i + 3);
            indices[j + 5] = (ushort)(i + 0);
        }

        return indices;
    }

    protected override ReadOnlySpan<TexturedVertex2D> CreateVertices(ReadOnlySpan<Vector2> vector)
    {
        Span<TexturedVertex2D> vertices = new TexturedVertex2D[4];

        vertices[0] = new()
        {
            Color = Color,
            TexCoord = Vector2.UnitY,
            Position = vector[0],
        };

        vertices[1] = new()
        {
            Color = Color,
            TexCoord = Vector2.Zero,
            Position = vector[1],
        };

        vertices[2] = new()
        {
            Color = Color,
            TexCoord = Vector2.UnitX,
            Position = vector[2],
        };

        vertices[3] = new()
        {
            Color = Color,
            TexCoord = Vector2.One,
            Position = vector[3],
        };

        return vertices;
    }

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
