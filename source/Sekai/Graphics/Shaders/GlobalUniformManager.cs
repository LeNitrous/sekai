// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Sekai.Extensions;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// Manages all <see cref="GlobalUniform"/>s which allow all shaders to have shared uniform values.
/// </summary>
public sealed class GlobalUniformManager : FrameworkObject
{
    private readonly Dictionary<string, GlobalUniform> uniforms = new();

    public GlobalUniformManager()
    {
        AddUniform(GlobalUniforms.Projection, Matrix4x4.Identity);
    }

    /// <summary>
    /// Creates a global uniform.
    /// </summary>
    /// <typeparam name="T">The uniform's type.</typeparam>
    /// <param name="name">The uniform's name.</param>
    /// <param name="value">The initial value of the uniform.</param>
    /// <exception cref="InvalidOperationException">Thrown when a uniform with the same name already exists</exception>
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

    /// <summary>
    /// Gets the uniform of a given name.
    /// </summary>
    /// <param name="name">The uniform's name to lookup.</param>
    /// <returns>The requested uniform.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no uniform with the given name.</exception>
    public GlobalUniform GetUniform(string name)
    {
        if (!uniforms.TryGetValue(name, out var uniform))
            throw new InvalidOperationException($@"Uniform ""{name}"" is not registered.");

        return uniform;
    }

    /// <summary>
    /// Gets the uniform of a given key.
    /// </summary>
    /// <param name="key">The uniform key.</param>
    /// <returns>The requested uniform.</returns>
    public GlobalUniform GetUniform(GlobalUniforms key) => GetUniform(key.GetDescription());

    /// <summary>
    /// Gets a strongly typed uniform of a given name.
    /// </summary>
    /// <typeparam name="T">The uniform's type.</typeparam>
    /// <param name="name">The uniform's name to lookup.</param>
    /// <returns>The strongly typed uniform.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no uniform with the given name.</exception>
    /// <exception cref="InvalidCastException">Thrown when the found uniform is not of a given type.</exception>
    public GlobalUniform<T> GetUniform<T>(string name)
        where T : unmanaged, IEquatable<T>
    {
        var uniform = GetUniform(name);

        if (uniform is not GlobalUniform<T> u)
            throw new InvalidCastException($"{name} is not a {typeof(GlobalUniform<T>)}.");

        return u;
    }

    /// <summary>
    /// Gets a strongly typed uniform of a given key.
    /// </summary>
    /// <typeparam name="T">The uniform's type.</typeparam>
    /// <param name="key">The uniform's key.</param>
    /// <returns>The strongly typed uniform.</returns>
    /// <exception cref="InvalidOperationException">Thrown when there is no uniform with the given name.</exception>
    /// <exception cref="InvalidCastException">Thrown when the found uniform is not of a given type.</exception>
    public GlobalUniform<T> GetUniform<T>(GlobalUniforms key)
        where T : unmanaged, IEquatable<T>
    {
        return GetUniform<T>(key.GetDescription());
    }

    /// <summary>
    /// Returns whether a uniform of a given name exists.
    /// </summary>
    public bool HasUniform(string name) => uniforms.ContainsKey(name);

    /// <summary>
    /// Gets a uniform of a given name.
    /// </summary>
    /// <param name="name">The uniform's name.</param>
    /// <param name="uniform">The requested uniform.</param>
    /// <returns>Returns <see cref="true"/> if the uniform exists. Otherwise, returns <see cref="false"/>.</returns>
    public bool TryGetUniform(string name, [NotNullWhen(true)] out GlobalUniform? uniform) => uniforms.TryGetValue(name, out uniform);

    /// <summary>
    /// Gets the strongly typed uniform of a given name.
    /// </summary>
    /// <typeparam name="T">THe uniform's type.</typeparam>
    /// <param name="name">The uniform's name.</param>
    /// <returns>The strongly typed uniform.</returns>
    /// <returns>Returns <see cref="true"/> if the uniform exists. Otherwise, returns <see cref="false"/>.</returns>
    public bool TryGetUniform<T>(string name, [NotNullWhen(true)] out GlobalUniform<T>? uniform)
        where T : unmanaged, IEquatable<T>
    {
        try
        {
            uniform = GetUniform<T>(name);
            return true;
        }
        catch
        {
            uniform = null;
            return false;
        }
    }
}
