// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Contexts;

/// <summary>
/// A context providing a <see cref="GLContext"/>.
/// </summary>
public interface IGLContextSource
{
    /// <summary>
    /// The GL context.
    /// </summary>
    GLContext Context { get; }
}
