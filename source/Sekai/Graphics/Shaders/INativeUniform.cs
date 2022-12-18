// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Shaders;

/// <summary>
/// A uniform owned by a <see cref="INativeShader"/>
/// </summary>
public interface INativeUniform : IUniform
{
    /// <summary>
    /// The uniform offset.
    /// </summary>
    int Offset { get; }

    /// <summary>
    /// The shader that declared this uniform.
    /// </summary>
    INativeShader Owner { get; }

    /// <summary>
    /// Updates the uniform's value.
    /// </summary>
    void Update();
}
