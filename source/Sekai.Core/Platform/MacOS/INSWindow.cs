// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.MacOS;

/// <summary>
/// An interface for NS windows.
/// </summary>
public interface INSWindow
{
    /// <summary>
    /// The NS window handle.
    /// </summary>
    nint Handle { get; }
}
