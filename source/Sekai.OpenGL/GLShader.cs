// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sekai.Graphics.Shaders;
using Silk.NET.OpenGL;
using FrameworkShaderType = Sekai.Graphics.Shaders.ShaderType;

namespace Sekai.OpenGL;

internal class GLShader : GLResource, INativeShader
{
    public FrameworkShaderType Type { get; }
    public IReadOnlyList<INativeUniform> Uniforms => uniforms;

    private readonly uint programId;
    private readonly uint[] shaderIds;
    private readonly List<INativeUniform> uniforms = new();

    public GLShader(GLGraphicsSystem context, string vert, string frag)
        : base(context)
    {
        Type = FrameworkShaderType.Graphic;

        shaderIds = new[]
        {
            compileShader(vert, GLEnum.VertexShader),
            compileShader(frag, GLEnum.FragmentShader)
        };

        programId = createProgram(shaderIds);

        setupUniforms();
    }

    public GLShader(GLGraphicsSystem context, string comp)
        : base(context)
    {
        Type = FrameworkShaderType.Compute;

        shaderIds = new[]
        {
            compileShader(comp, GLEnum.ComputeShader)
        };

        programId = createProgram(shaderIds);

        setupUniforms();
    }

    public void Update()
    {
        foreach (var uniform in uniforms)
            uniform.Update();
    }

    private uint compileShader(string code, GLEnum type)
    {
        uint id = GL.CreateShader(type);
        GL.ShaderSource(id, code);
        GL.CompileShader(id);

        string log;

        if (!string.IsNullOrEmpty(log = GL.GetShaderInfoLog(id)))
            throw new Exception($"Failed to compile shader: {log}");

        return id;
    }

    private uint createProgram(params uint[] shaders)
    {
        uint id = GL.CreateProgram();

        for (int i = 0; i < shaders.Length; i++)
            GL.AttachShader(id, shaders[i]);

        GL.LinkProgram(id);

        GL.GetProgram(id, GLEnum.LinkStatus, out int status);

        if (status == 0)
            throw new Exception($"Failed to link shader: {GL.GetProgramInfoLog(id)}");

        return id;
    }

    private void setupUniforms()
    {
        GL.GetProgram(programId, GLEnum.ActiveUniforms, out int count);

        for (uint i = 0; i < count; i++)
        {
            GL.GetActiveUniform(programId, i, 255, out _, out _, out UniformType type, out string name);

            switch (type)
            {
                case UniformType.Int:
                    uniforms.Add(new GLUniform<int>(this, name, (int)i));
                    break;

                case UniformType.UnsignedInt:
                    uniforms.Add(new GLUniform<uint>(this, name, (int)i));
                    break;

                case UniformType.Float:
                    uniforms.Add(new GLUniform<float>(this, name, (int)i));
                    break;

                case UniformType.Double:
                    uniforms.Add(new GLUniform<double>(this, name, (int)i));
                    break;

                case UniformType.FloatVec2:
                    uniforms.Add(new GLUniform<Vector2>(this, name, (int)i));
                    break;

                case UniformType.FloatVec3:
                    uniforms.Add(new GLUniform<Vector3>(this, name, (int)i));
                    break;

                case UniformType.FloatVec4:
                    uniforms.Add(new GLUniform<Vector4>(this, name, (int)i));
                    break;

                case UniformType.FloatMat4:
                    uniforms.Add(new GLUniform<Matrix4x4>(this, name, (int)i));
                    break;
            }
        }
    }

    internal unsafe void UpdateUniform<T>(GLUniform<T> uniform)
        where T : unmanaged, IEquatable<T>
    {
        var prev = Context.CurrentShader;
        Context.SetShader(this);

        switch (uniform)
        {
            case GLUniform<int> i:
                GL.Uniform1(uniform.Offset, 1, (int*)Unsafe.AsPointer(ref i.GetValueByRef()));
                break;

            case GLUniform<uint> u:
                GL.Uniform1(uniform.Offset, 1, (uint*)Unsafe.AsPointer(ref u.GetValueByRef()));
                break;

            case GLUniform<float> f:
                GL.Uniform1(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref f.GetValueByRef()));
                break;

            case GLUniform<double> d:
                GL.Uniform1(uniform.Offset, 1, (double*)Unsafe.AsPointer(ref d.GetValueByRef()));
                break;

            case GLUniform<Vector2> v:
                GL.Uniform2(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref v.GetValueByRef()));
                break;

            case GLUniform<Vector3> v:
                GL.Uniform3(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref v.GetValueByRef()));
                break;

            case GLUniform<Vector4> v:
                GL.Uniform4(uniform.Offset, 1, (float*)Unsafe.AsPointer(ref v.GetValueByRef()));
                break;

            case GLUniform<Matrix4x4> m:
                GL.UniformMatrix4(uniform.Offset, 1, false, (float*)Unsafe.AsPointer(ref m.GetValueByRef()));
                break;

            default:
                throw new NotSupportedException(@"Uniform is not supported.");
        }

        Context.SetShader(prev);
    }

    protected override void Destroy()
    {
        uniforms.Clear();

        for (int i = 0; i < shaderIds.Length; i++)
        {
            GL.DetachShader(programId, shaderIds[i]);
            GL.DeleteShader(shaderIds[i]);
        }

        GL.DeleteProgram(programId);
    }

    public static implicit operator uint(GLShader shader) => shader.programId;
}
