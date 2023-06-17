// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed unsafe class GLShader : Shader
{
    public override ShaderStage Stages { get; }

    private bool isDisposed;
    private readonly uint handle;
    private static uint bound_handle;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLShader(GL gl, ShaderCode[] attachments)
    {
        GL = gl;
        handle = GL.CreateProgram();

        Span<uint> shaderHandles = stackalloc uint[attachments.Length];

        for (int i = 0; i < attachments.Length; i++)
        {
            Stages |= attachments[i].Stage;

            var type = attachments[i].Stage.AsType();
            shaderHandles[i] = GL.CreateShader(type);

            string code = attachments[i].GetText(ShaderLanguage.GLSL)!;
            GL.ShaderSource(shaderHandles[i], code);

            GL.CompileShader(shaderHandles[i]);
            GL.GetShader(shaderHandles[i], ShaderParameterName.CompileStatus, out int status);

            if (status != (int)GLEnum.True)
            {
                throw new ShaderCompilationException($"Failed to compile shader: {GL.GetShaderInfoLog(shaderHandles[i])}");
            }

            GL.AttachShader(handle, shaderHandles[i]);
        }

        GL.LinkProgram(handle);
        GL.GetProgram(handle, ProgramPropertyARB.LinkStatus, out int link);

        if (link != (int)GLEnum.True)
        {
            throw new ShaderCompilationException($"Failed to compile shader program: {GL.GetProgramInfoLog(handle)}");
        }

        for (int i = 0; i < shaderHandles.Length; i++)
        {
            GL.DetachShader(handle, shaderHandles[i]);
            GL.DeleteShader(shaderHandles[i]);
        }
    }

    public void Bind()
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(nameof(GLShader));
        }

        if (bound_handle == handle)
        {
            return;
        }

        GL.UseProgram(handle);

        bound_handle = handle;
    }

    ~GLShader()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        GL.DeleteProgram(handle);

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
