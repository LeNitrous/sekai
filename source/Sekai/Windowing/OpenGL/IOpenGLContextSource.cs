// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Windowing.OpenGL;

/// <summary>
/// An interface for windowing systems capable of providing OpenGL.
/// </summary>
public interface IOpenGLContextSource
{
    /// <summary>
    /// The OpenGL context.
    /// </summary>
    IOpenGLContext GL { get; }
}
