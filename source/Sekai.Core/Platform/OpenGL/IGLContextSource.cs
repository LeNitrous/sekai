// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.OpenGL;

/// <summary>
/// An interface that handles OpenGL contexts.
/// </summary>
public interface IGLContextSource
{
    /// <summary>
    /// Gets an address to a GL function pointer by name.
    /// </summary>
    /// <param name="proc"></param>
    /// <returns>The address to a GL function pointer.</returns>
    nint GetProcAddress(string proc);

    /// <summary>
    /// Creates a GL context.
    /// </summary>
    /// <returns>The created GL context.</returns>
    nint CreateContext();

    /// <summary>
    /// Makes the GL context current on the calling thread.
    /// </summary>
    /// <param name="context">The context to make current.</param>
    void MakeCurrent(nint context);

    /// <summary>
    /// Gets the current GL context.
    /// </summary>
    /// <returns>The current context on the calling thread.</returns>
    nint GetCurrentContext();

    /// <summary>
    /// Clears the GL context on the calling thread.
    /// </summary>
    void ClearCurrentContext();

    /// <summary>
    /// Deletes the GL context.
    /// </summary>
    /// <param name="context">The context to delete.</param>
    void DeleteContext(nint context);

    /// <summary>
    /// Swaps the front and back buffers associated with the current GL context.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// Sets the synchronization behavior of the current GL context.
    /// </summary>
    /// <param name="sync"><see langword="true"/> to enable syncing. Otherwise, use <see langword="false"/>.</param>
    void SetSyncToVerticalBlank(bool sync);
}
