// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces.OpenGL;

/// <summary>
/// An interface representing a GL context.
/// </summary>
public interface IGLContext
{
    /// <summary>
    /// The source that created this context.
    /// </summary>
    IGLContextSource Source { get; }
}
