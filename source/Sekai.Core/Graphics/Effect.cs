// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Veldrid;

namespace Sekai.Graphics;

/// <summary>
/// Determines how a renderable should be drawn onto the screen.
/// </summary>
public sealed partial class Effect : IDisposable
{
    /// <summary>
    /// The effect kind.
    /// </summary>
    public EffectKind Kind { get; }

    /// <summary>
    /// An enumeration of all parameter keys.
    /// </summary>
    public IEnumerable<string> Parameters => parameters.Keys;

    /// <summary>
    /// Gets the effect's block size.
    /// </summary>
    internal int Size { get; }

    private bool isDisposed;
    private readonly Shader[] shaders;
    private readonly Reflection reflect;
    private readonly Dictionary<string, EffectParameter> parameters = new();

    private Effect(EffectKind kind, Dictionary<string, EffectParameter> parameters, Shader[] shaders, Reflection reflect, int size)
    {
        Kind = kind;
        Size = size;
        this.shaders = shaders;
        this.reflect = reflect;
        this.parameters = parameters;
    }

    /// <summary>
    /// Gets whether the effect has the named parameter.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns><see langword="true"/> if the effect has the parameter. Otherwise, returns <see langword="false"/>.</returns>
    internal bool HasParameter(string name)
    {
        return TryGetParameter(name, out _);
    }

    /// <summary>
    /// Gets whether the effect has the named parameter of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns><see langword="true"/> if the effect has the parameter of type <typeparamref name="T"/>. Otherwise, returns <see langword="false"/>.</returns>
    internal bool HasParameter<T>(string name)
        where T : EffectParameter
    {
        return TryGetParameter<T>(name, out _);
    }

    /// <summary>
    /// Gets the named effect parameter.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <returns>The effect parameter.</returns>
    /// <exception cref="EffectParameterNotFoundException">Thrown when the parameter does not exist.</exception>
    internal EffectParameter GetParameter(string name)
    {
        if (!parameters.ContainsKey(name))
        {
            throw new EffectParameterNotFoundException($"The parameter {name} is not found in this effect.");;
        }

        return parameters[name];
    }

    /// <summary>
    /// Gets the named effect parameter of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <returns><The effect parameter of type <typeparamref name="T"/>./returns>
    /// <exception cref="EffectParameterNotFoundException">Thrown when the parameter does not exist.</exception>
    internal T GetParameter<T>(string name)
        where T : EffectParameter
    {
        var param = GetParameter(name);

        if (param is not T paramT)
        {
            throw new EffectParameterNotFoundException($"The parameter {name} is not found in this effect.");
        }

        return paramT;
    }

    /// <summary>
    /// Gets the named effect parameter.
    /// </summary>
    /// <param name="name">The parameter name.</param>
    /// <param name="parameter">When this method returns <see langword="true"/>, it contains the effect parameter. If the method returns <see langword="false"/>, this value is <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the effect has the parameter. Otherwise, returns <see langword="false"/>.</returns>
    internal bool TryGetParameter(string name, [NotNullWhen(true)] out EffectParameter? parameter)
    {
       return parameters.TryGetValue(name, out parameter);
    }

    /// <summary>
    /// Gets the named effect parameter of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The parameter type.</typeparam>
    /// <param name="name">The parameter name.</param>
    /// <param name="parameter">When this method returns <see langword="true"/>, it contains the effect parameter of type <typeparamref name="T"/>. If the method returns <see langword="false"/>, this value is <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the effect has the parameter. Otherwise, returns <see langword="false"/>.</returns>
    internal bool TryGetParameter<T>(string name, [NotNullWhen(true)] out T? parameter)
        where T : EffectParameter
    {
        bool contains = TryGetParameter(name, out var param);

        if (!contains)
        {
            parameter = null;
            return false;
        }

        parameter = param as T;
        return parameter is not null;
    }

    /// <summary>
    /// Creates a new effect.
    /// </summary>
    /// <param name="device">The graphics device.</param>
    /// <param name="code">The effect code.</param>
    /// <returns>A new effect.</returns>
    public static Effect Create(GraphicsDevice device, string code)
    {
        var shaders = process(device, code, out var kind, out var reflect);
        var mat = reflect.Buffers.Single(static r => r.Name == buffer_user);
        var inf = reflect.Types[mat.Type];
        var parameters = new Dictionary<string, EffectParameter>();

        for (int i = 0; i < inf.Members.Length; i++)
        {
            var member = inf.Members[i];

            int nextOffset = (i + i) <= inf.Members.Length ? inf.Members[i + 1].Offset : mat.Size;
            int size = nextOffset - member.Offset;

            parameters.Add(member.Name, new EffectValueParameter(member.Name, size, member.Offset));
        }

        foreach (var texture in reflect.Textures)
        {
            string name = texture.Name.Replace("g_internal_TEXTURE_", string.Empty);
            parameters.Add(name, new EffectOpaqueParameter(name));
        }

        return new(kind, parameters, shaders, reflect, mat.Size);
    }

    ~Effect()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        foreach (var shader in shaders)
        {
            shader.Dispose();
        }

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
