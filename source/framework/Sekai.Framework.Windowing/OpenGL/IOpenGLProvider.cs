// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Windowing.OpenGL;

public interface IOpenGLProvider
{
    /// <summary>
    /// The OpenGL handle.
    /// </summary>
    nint Handle { get; }

    /// <summary>
    /// Gets the pointer to a certain function.
    /// </summary>
    nint GetProcAddress(string name);

    /// <summary>
    /// M>ake the given OpenGL context current on the calling thread.
    /// </summary>
    void MakeCurrent(nint context);

    /// <summary>
    /// Retrieve sthe calling thread's active OpenGL context.
    /// </summary>
    nint GetCurrentContext();

    /// <summary>
    /// Clears the current OpenGL Context.
    /// </summary>
    void ClearCurrentContext();

    /// <summary>
    /// Delete the given context.
    /// </summary>
    void DeleteContext(nint context);

    /// <summary>
    /// Swap the main back buffer associated with the OpenGL context.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// Set the synchronization behavior of the OpenGL context
    /// </summary>
    void SetSyncToVerticalBlank(bool sync);
}
