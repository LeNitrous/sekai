// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;
using Sekai.Assets;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// A program that is executed at a specified stage on the GPU.
/// </summary>
public sealed partial class Shader : ServiceableGraphicsObject<NativeShader>, IAsset
{
    /// <summary>
    /// The shader type.
    /// </summary>
    public ShaderType Type => Native.Type;

    /// <summary>
    /// The shader code.
    /// </summary>
    public readonly string Code;

    /// <summary>
    /// Creates a new shader from code.
    /// </summary>
    public Shader(string code)
        : base(context => context.CreateShader(code))
    {
        Code = code;
    }

    /// <summary>
    /// Retrieves a declared uniform from this shader.
    /// </summary>
    public IUniform<T> GetUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        var uniform = Native.Uniforms.FirstOrDefault(u => u.Name == name);

        if (uniform is null)
            throw new ArgumentException($@"There is no uniform named ""{name}"".", nameof(name));

        if (uniform is not IUniform<T> uniformValue)
            throw new InvalidCastException($@"Uniform is not a type of ""{typeof(T)}"".");

        return uniformValue;
    }
}
