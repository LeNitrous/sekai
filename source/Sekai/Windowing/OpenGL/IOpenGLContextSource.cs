// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Windowing.OpenGL;

/// <summary>
/// An interface for windowing systems capable of providing OpenGL contexts.
/// </summary>
public interface IOpenGLContextSource
{
    /// <summary>
    /// Gets the pointer to an OpenGL function.
    /// </summary>
    nint GetProcAddress(string name);

    /// <summary>
    /// Clears the current context on the calling thread.
    /// </summary>
    void ClearCurrentContext();

    /// <summary>
    /// Swap the main back buffer.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// Set the synchronization behavior.
    /// </summary>
    void SetSyncToVerticalBlank(bool sync);

    /// <summary>
    /// Creates an OpenGL context.
    /// </summary>
    IOpenGLContext CreateContext();

    /// <summary>
    /// Retrieves the calling thread's active context.
    /// </summary>
    IOpenGLContext? GetCurrentContext();
}
