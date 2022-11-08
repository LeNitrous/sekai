// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Shaders;

/// <summary>
/// Represents a shader uniform.
/// </summary>
public interface IUniform
{
    /// <summary>
    /// The uniform name.
    /// </summary>
    string Name { get; }
}

public interface IUniform<T> : IUniform
    where T : unmanaged, IEquatable<T>
{
    /// <summary>
    /// The uniform's value.
    /// </summary>
    T Value { get; set; }
}
