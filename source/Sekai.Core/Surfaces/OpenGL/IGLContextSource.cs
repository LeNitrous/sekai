// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces.OpenGL;

/// <summary>
/// An interface for surfaces that are sources for an <see cref="IGLContext"/>.
/// </summary>
public interface IGLContextSource
{
    /// <summary>
    /// Swaps the main back buffer.
    /// </summary>
    void SwapBuffers();

    /// <summary>
    /// Gets the pointer to a GL function.
    /// </summary>
    /// <param name="name">The function name.</param>
    /// <returns>The pointer to a GL function.</returns>
    nint GetProcAddress(string name);

    /// <summary>
    /// Sets the synchronization behavior.
    /// </summary>
    /// <param name="sync">The synchronization state to set.</param>
    void SetSyncToVerticalBlank(bool sync);

    /// <summary>
    /// Creates a GL context.
    /// </summary>
    /// <returns>The created context.</returns>
    IGLContext CreateContext();

    /// <summary>
    /// Makes the <paramref name="context"/> the current on the calling thread.
    /// </summary>
    /// <param name="context">The context to set as current.</param>
    void MakeCurrent(IGLContext context);

    /// <summary>
    /// Clears the context on the calling thread.
    /// </summary>
    void ClearCurrent();
}
