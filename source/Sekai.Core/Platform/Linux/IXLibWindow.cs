// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.Linux;

/// <summary>
/// An interface for xlib windows.
/// </summary>
public interface IXLibWindow
{
    /// <summary>
    /// Gets whether the windowing system uses xlib.
    /// </summary>
    bool IsXLib { get; }

    /// <summary>
    /// The xlib display handle.
    /// </summary>
    nint Display { get; }

    /// <summary>
    /// The xlib surface handle.
    /// </summary>
    nint Surface { get; }
}
