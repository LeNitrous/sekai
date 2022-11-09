// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Runtime.InteropServices;
using Sekai;
using Sekai.Graphics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;

namespace Triangle;

public class TriangleGame : Game
{
    private Shader shd = null!;
    private Buffer<short> ebo = null!;
    private Buffer<Vertex2D> vbo = null!;
    private static readonly string shader = @"
attrib vec2 Position;

vec4 vert()
{
    return vec4(Position.x, Position.y, 0.0f, 1.0f);
}

vec4 frag()
{
    return vec4(1.0f, 1.0f, 1.0f, 1.0f);
}
";

    protected override void Load()
    {
        shd = new Shader(shader);

        ebo = new Buffer<short>(3);
        ebo.SetData(new short[] { 0, 1, 2 });

        vbo = new Buffer<Vertex2D>(3);
        vbo.SetData(new[]
        {
            new Vertex2D { Position = new Vector2(-0.5f, -0.5f) },
            new Vertex2D { Position = new Vector2(+0.5f, -0.5f) },
            new Vertex2D { Position = new Vector2(+0.0f, +0.5f) },
        });
    }

    protected override void Render()
    {
        shd.Bind();
        ebo.Bind();
        vbo.Bind();
        Graphics.Draw(3, PrimitiveTopology.Triangles);
    }

    protected override void Unload()
    {
        shd?.Dispose();
        ebo?.Dispose();
        vbo?.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex2D : IVertex, IVertex2D
    {
        [field: VertexMember("Position", 2)]
        public Vector2 Position { get; set; }
    }
}
