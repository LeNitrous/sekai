// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai;
using Sekai.Graphics;
using Sekai.Mathematics;

namespace SampleGame;

internal static class Program
{
    private static void Main()
    {
        Host.Run<Sample>(new HostOptions { Name = "Sample" });
    }
}

internal sealed class Sample : Game
{
    private Shader? shd;
    private InputLayout? layout;
    private GraphicsBuffer? vbo;

    public override void Load()
    {
        shd = Graphics.CreateShader
        (
            ShaderCode.From(shader_v_code, ShaderStage.Vertex),
            ShaderCode.From(shader_f_code, ShaderStage.Fragment)
        );

        ReadOnlySpan<Vector2> vertices = stackalloc Vector2[]
        {
            new(-0.5f, -0.5f),
            new(0.5f, -0.5f),
            new(0.0f, 0.5f),
        };

        vbo = Graphics.CreateBuffer<Vector2>(BufferType.Vertex, (uint)vertices.Length);
        vbo.SetData(vertices);

        layout = Graphics.CreateInputLayout
        (
            new InputLayoutDescription
            (
                new InputLayoutMember(2, false, InputLayoutFormat.Float)
            )
        );
    }

    public override void Draw()
    {
        Graphics.Clear(Color.CornflowerBlue);
        Graphics.SetShader(shd!);
        Graphics.SetVertexBuffer(vbo!, layout!);
        Graphics.Draw(PrimitiveType.TriangleList, 3);
    }

    public override void Unload()
    {
        shd?.Dispose();
        vbo?.Dispose();
    }

    private const string shader_v_code =
@"
#version 450

layout (location = 0) in vec2 a_position;

void main()
{
    gl_Position = vec4(a_position.x, a_position.y, 0.0, 1.0);
}
";

    private const string shader_f_code =
@"
#version 450

layout (location = 0) out vec4 v_color;

void main()
{
    v_color = vec4(1.0, 0.5, 0.2, 1.0);
}
";
}
