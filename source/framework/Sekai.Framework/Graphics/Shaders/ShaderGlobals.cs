// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sekai.Framework.Graphics.Shaders;

public sealed class ShaderGlobals : FrameworkObject
{
    private readonly Dictionary<string, GlobalUniform> uniforms = new();

    public ShaderGlobals()
    {
        AddUniform<Matrix4x4>("g_Matrix");
    }

    public void AddUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        if (HasUniform(name))
            throw new InvalidOperationException($@"Uniform ""{name}"" is already registered.");

        uniforms.Add(name, new GlobalUniform<T>(name));
    }

    public GlobalUniform GetUniform(string name)
    {
        if (!uniforms.TryGetValue(name, out var uniform))
            throw new InvalidOperationException($@"Uniform ""{name}"" is not registered..");

        return uniform;
    }

    public GlobalUniform<T> GetUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        var uniform = GetUniform(name);

        if (uniform is not GlobalUniform<T> u)
            throw new InvalidOperationException();

        return u;
    }

    public bool HasUniform(string name) => uniforms.ContainsKey(name);
}
