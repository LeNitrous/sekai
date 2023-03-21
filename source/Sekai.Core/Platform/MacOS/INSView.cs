// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.MacOS;

/// <summary>
/// An interface for NS views.
/// </summary>
public interface INSView
{
    /// <summary>
    /// The NS view handle.
    /// </summary>
    nint Handle { get; }
}
