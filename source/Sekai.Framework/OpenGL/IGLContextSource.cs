// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.OpenGL;

/// <summary>
/// An interface for GL context sources.
/// </summary>
public interface IGLContextSource
{
    /// <summary>
    /// The GL context.
    /// </summary>
    GLContext Context { get; }
}
