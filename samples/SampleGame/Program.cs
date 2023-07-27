// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai;
using Sekai.GLFW;
using Sekai.Graphics;
using Sekai.Mathematics;
using Sekai.OpenAL;
using Sekai.OpenGL;

namespace SampleGame;

internal static class Program
{
    private static void Main()
    {
        var options = new GameOptions();
        options.UseOpenAL();
        options.UseOpenGL();
        options.Logger.AddConsole();

        if (RuntimeInfo.IsDesktop)
        {
            options.UseGLFW();
            options.Window.Title = "Sample";
        }

        var game = new Sample(options);
        game.Run();
    }
}

internal sealed class Sample : Game
{
    private Shader? shd;
    private InputLayout? layout;
    private GraphicsBuffer? vbo;

    public Sample(GameOptions options)
        : base(options)
    {
    }

    protected override void Load()
    {
        shd = Graphics.CreateShader
        (
            ShaderCode.From(shader_v_code, ShaderStage.Vertex),
            ShaderCode.From(shader_f_code, ShaderStage.Fragment)
        );

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

    protected override void Draw()
    {
        Graphics.Clear(Color.CornflowerBlue);
        Graphics.SetShader(shd!);
        Graphics.SetVertexBuffer(vbo!, layout!);
        Graphics.Draw(PrimitiveType.TriangleList, 3);
    }

    protected override void Unload()
    {
        shd?.Dispose();
        vbo?.Dispose();
    }

    private static readonly Vector2[] vertices = new Vector2[]
    {
        new(-0.5f, -0.5f),
        new(0.5f, -0.5f),
        new(0.0f, 0.5f),
    };

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
