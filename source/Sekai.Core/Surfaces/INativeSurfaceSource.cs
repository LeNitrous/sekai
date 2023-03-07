// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces;

/// <summary>
/// An interface for surfaces that are backed natively.
/// </summary>
public interface INativeSurfaceSource
{
    /// <summary>
    /// The surface's native handle.
    /// </summary>
    INativeSurface Native { get; }
}
