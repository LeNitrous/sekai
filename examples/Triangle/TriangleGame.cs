// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using System.Runtime.InteropServices;
using Sekai;
using Sekai.Graphics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;

namespace Triangle;

public class TriangleGame : Game
{
    private Shader shd = null!;
    private Buffer<short> ebo = null!;
    private Buffer<Vertex2D> vbo = null!;
    private Texture tex = null!;
    private static readonly string shader = @"
attrib vec2 a_Position;
extern mat4 g_ProjMatrix;
extern sampler2D Texture;

vec4 vert()
{
    return g_ProjMatrix * vec4(a_Position.x, a_Position.y, 0.0f, 1.0f);
}

vec4 frag()
{
    return texture(Texture, vec2(0.0f, 0.0f));
}
";

    public override void Load()
    {
        shd = new Shader(shader);

        ebo = new Buffer<short>(3);
        ebo.SetData(new short[] { 0, 1, 2 });

        vbo = new Buffer<Vertex2D>(3);
        vbo.SetData(new[]
        {
            new Vertex2D { Position = new Vector2(-0.5f, -0.5f) * 50 },
            new Vertex2D { Position = new Vector2(+0.5f, -0.5f) * 50 },
            new Vertex2D { Position = new Vector2(+0.0f, +0.5f) * 50 },
        });

        tex = Texture.New2D(1, 1, PixelFormat.R8_G8_B8_A8_UNorm_SRgb);
        tex.SetData(new byte[] { 0, 0, 255, 255 }, 0, 0, 0, 1, 1, 1, 0, 0);
    }

    public override void Render()
    {
        Graphics.PushProjectionMatrix(Matrix4x4.CreateOrthographic(1280, 720, -1 ,1));
        tex.Bind();
        shd.Bind();
        ebo.Bind();
        vbo.Bind();
        Graphics.Draw(3, PrimitiveTopology.Triangles);
    }

    public override void Unload()
    {
        shd?.Dispose();
        ebo?.Dispose();
        vbo?.Dispose();
        tex?.Dispose();
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex2D : IVertex, IVertex2D
    {
        [field: VertexMember("Position", 2)]
        public Vector2 Position { get; set; }
    }
}
