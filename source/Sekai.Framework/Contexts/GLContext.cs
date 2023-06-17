// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Contexts;

/// <summary>
/// Provides methods for various OpenGL context operations.
/// </summary>
public readonly struct GLContext
{
    /// <summary>
    /// The handle to the GL context.
    /// </summary>
    public readonly nint Handle;

    /// <summary>
    /// Gets an address to a GL function pointer by name.
    /// </summary>
    public readonly Func<string, nint> GetProcAddress;

    /// <summary>
    /// Makes the GL context current on the calling thread.
    /// </summary>
    public readonly Action MakeCurrent;

    /// <summary>
    /// Clears the GL context on the calling thread.
    /// </summary>
    public readonly Action Clear;

    /// <summary>
    /// Swaps the front and back buffers associated with the current GL context.
    /// </summary>
    public readonly Action SwapBuffers;

    /// <summary>
    /// Sets the synchronization behavior of the current GL context.
    /// </summary>
    public readonly Action<int> SwapInterval;

    public GLContext(nint context, Func<string, nint> getProcAddress, Action makeCurrent, Action clear, Action swapBuffers, Action<int> swapInterval)
    {
        Handle = context;
        GetProcAddress = getProcAddress;
        MakeCurrent = makeCurrent;
        Clear = clear;
        SwapBuffers = swapBuffers;
        SwapInterval = swapInterval;
    }
}
