// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Windowing.OpenGL;

/// <summary>
/// An OpenGL context.
/// </summary>
public interface IOpenGLContext : IDisposable
{
    /// <summary>
    /// Makes itself the current context on the calling thread.
    /// </summary>
    void MakeCurrent();
}
