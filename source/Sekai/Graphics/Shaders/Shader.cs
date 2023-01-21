// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Allocation;
using Sekai.Assets;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// A program that is executed at a specified stage on the GPU.
/// </summary>
public sealed partial class Shader : GraphicsObject, IAsset
{
    /// <summary>
    /// The shader type.
    /// </summary>
    public ShaderType Type => Native.Type;

    /// <summary>
    /// The shader code.
    /// </summary>
    public readonly string Code;

    [Resolved]
    private ShaderTranspiler transpiler { get; set; } = null!;

    [Resolved]
    private ShaderUniformManager manager { get; set; } = null!;

    private readonly Dictionary<string, IUniform> uniforms = new();

    internal readonly NativeShader Native;

    /// <summary>
    /// Creates a new shader from code.
    /// </summary>
    public Shader(string code)
    {
        Native = Graphics.CreateShader(transpiler.Transpile(Code = code));
        manager.Link(Native);

        foreach (var uniform in Native.Uniforms)
            uniforms.Add(uniform.Name, uniform);
    }

    /// <summary>
    /// Retrieves a declared uniform from this shader.
    /// </summary>
    public IUniform<T> GetUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        if (manager.Contains(name))
            throw new ArgumentException(@"Uniform is a global uniform.", nameof(name));

        if (!uniforms.TryGetValue(name, out var uniform))
            throw new ArgumentException($@"There is no uniform named ""{name}"".", nameof(name));

        if (uniform is not IUniform<T> uniformValue)
            throw new InvalidCastException($@"Uniform is not a type of ""{typeof(T)}"".");

        return uniformValue;
    }

    protected override void DestroyGraphics()
    {
        manager.Unlink(Native);
        Native.Dispose();
    }
}
