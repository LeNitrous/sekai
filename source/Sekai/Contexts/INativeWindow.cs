// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Contexts;

/// <summary>
/// A context providing a native window.
/// </summary>
public interface INativeWindow
{
    /// <summary>
    /// The native window info.
    /// </summary>
    NativeWindowInfo Native { get; }
}
