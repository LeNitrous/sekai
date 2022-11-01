// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Windowing.OpenGL;

/// <summary>
/// An interface for windowing systems capable of providing OpenGL APIs.
/// </summary>
public interface IOpenGLProvider : IDisposable
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
    /// Make the given context current on the calling thread.
    /// </summary>
    void MakeCurrent(nint context);

    /// <summary>
    /// Make the main context the current.
    /// </summary>
    void MakeCurrent();

    /// <summary>
    /// Retrieves the calling thread's active context.
    /// </summary>
    nint GetCurrentContext();

    /// <summary>
    /// Creates an context.
    /// </summary>
    nint CreateContext();

    /// <summary>
    /// Clears the current Context.
    /// </summary>
    void ClearCurrentContext();

    /// <summary>
    /// Delete the given context.
    /// </summary>
    void DeleteContext(nint context);

    /// <summary>
    /// Swap the main back buffer associated with the context.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// Set the synchronization behavior of the context
    /// </summary>
    void SetSyncToVerticalBlank(bool sync);
}
