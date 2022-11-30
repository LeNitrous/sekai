// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Extensions;

namespace Sekai.Graphics.Shaders;

public sealed class GlobalUniformManager : FrameworkObject
{
    private readonly Dictionary<string, GlobalUniform> uniforms = new();

    public GlobalUniformManager()
    {
        AddUniform(GlobalUniforms.View, Matrix4x4.Identity);
        AddUniform(GlobalUniforms.Projection, Matrix4x4.Identity);
    }

    public void AddUniform<T>(string name, T value)
        where T : unmanaged, IEquatable<T>
    {
        if (HasUniform(name))
            throw new InvalidOperationException($@"Uniform ""{name}"" is already registered.");

        uniforms.Add(name, new GlobalUniform<T>(name) { Value = value });
    }

    internal void AddUniform<T>(GlobalUniforms key, T value)
        where T : unmanaged, IEquatable<T>
    {
        AddUniform(key.GetDescription(), value);
    }

    public GlobalUniform GetUniform(string name)
    {
        if (!uniforms.TryGetValue(name, out var uniform))
            throw new InvalidOperationException($@"Uniform ""{name}"" is not registered.");

        return uniform;
    }

    public GlobalUniform GetUniform(GlobalUniforms key)
    {
        return GetUniform(key.GetDescription());
    }

    public GlobalUniform<T> GetUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        var uniform = GetUniform(name);

        if (uniform is not GlobalUniform<T> u)
            throw new InvalidOperationException();

        return u;
    }

    public GlobalUniform<T> GetUniform<T>(GlobalUniforms key)
        where T : unmanaged, IEquatable<T>
    {
        return GetUniform<T>(key.GetDescription());
    }

    public bool HasUniform(string name) => uniforms.ContainsKey(name);
}
