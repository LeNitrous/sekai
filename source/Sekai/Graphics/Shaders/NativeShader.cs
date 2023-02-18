// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Graphics.Shaders;

/// <summary>
/// A shader graphics device.
/// </summary>
public abstract class NativeShader : DisposableObject
{
    /// <summary>
    /// The shader type.
    /// </summary>
    public abstract ShaderType Type { get; }

    /// <summary>
    /// The uniforms declared in this shader.
    /// </summary>
    public abstract IReadOnlyList<IUniform> Uniforms { get; }
}
