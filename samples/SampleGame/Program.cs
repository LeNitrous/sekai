// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using Sekai;
using Sekai.Desktop;
using Sekai.Framework;
using Sekai.Framework.Audio;
using Sekai.Framework.Contexts;
using Sekai.Framework.Graphics;
using Sekai.Framework.Mathematics;
using Sekai.Framework.Storages;
using Sekai.Framework.Windowing;
using Sekai.OpenAL;
using Sekai.OpenGL;

namespace SampleGame;

internal static class Program
{
    private static void Main()
    {
        if (RuntimeInfo.IsDesktop)
        {
            var host = new SampleHost();
            var game = new Sample();
            host.Run(game);
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }
}

internal sealed class SampleHost : DesktopHost
{
    protected override AudioDevice CreateAudio()
    {
        return new ALAudioDevice();
    }

    protected override GraphicsDevice CreateGraphics(IWindow window)
    {
        if (window is not IGLContextSource source)
        {
            throw new ArgumentException("Window does not provide a GL context.", nameof(window));
        }

        return new GLGraphicsDevice(source.Context);
    }

    protected override Storage CreateStorage()
    {
        return new MemoryStorage();
    }
}

internal sealed class Sample : Game
{
    private Shader? shd;
    private GraphicsBuffer? vbo;
    private readonly VertexLayout layout = new(new VertexMember(2, false, VertexMemberFormat.Float));

    public override void Load()
    {
        shd = Graphics!.CreateShader
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

        vbo = Graphics.CreateBuffer(BufferType.Vertex, vertices);
    }

    public override void Draw()
    {
        Graphics!.Clear(Color.CornflowerBlue);
        Graphics.SetShader(shd!);
        Graphics.SetVertexLayout(layout);
        Graphics.SetVertexBuffer(vbo!);
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
