// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Extensions;

namespace Sekai.Graphics.Shaders;

internal sealed class ShaderUniformManager : DependencyObject
{
    public GlobalUniform this[GlobalUniforms key] => uniforms[(int)key];

    private readonly GlobalUniform[] uniforms;
    private readonly Dictionary<string, GlobalUniform> uniformMap = new();

    public ShaderUniformManager()
    {
        uniforms = new GlobalUniform[Enum.GetValues<GlobalUniforms>().Length];

        add(GlobalUniforms.Projection, Matrix4x4.Identity);
        add(GlobalUniforms.Model, Matrix4x4.Identity);
        add(GlobalUniforms.View, Matrix4x4.Identity);

        void add<T>(GlobalUniforms key, T defaultValue = default)
            where T : unmanaged, IEquatable<T>
        {
            string name = key.GetDescription();
            uniformMap.Add(name, uniforms[(int)key] = new GlobalUniform<T>(name, defaultValue));
        }
    }

    public void Reset(GlobalUniforms key)
    {
        uniforms[(int)key].Reset();
    }

    public void Link(NativeShader shader)
    {
        foreach (var uniform in shader.Uniforms)
        {
            if (!uniformMap.TryGetValue(uniform.Name, out var uniformGlobal))
                continue;

            uniformGlobal.Attach(uniform);
        }
    }

    public void Unlink(NativeShader shader)
    {
        foreach (var uniform in shader.Uniforms)
        {
            if (!uniformMap.TryGetValue(uniform.Name, out var uniformGlobal))
                continue;

            uniformGlobal.Detach(uniform);
        }
    }

    public bool Contains(string name) => uniformMap.ContainsKey(name);

    protected override void Destroy()
    {
        uniforms.AsSpan().Clear();
        uniformMap.Clear();
    }
}
