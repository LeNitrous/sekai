// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform;

/// <summary>
/// An interface for objects that can host one or more of an <see cref="IWindow"/>.
/// </summary>
public interface IWindowHost
{
    /// <summary>
    /// Creates a new child window.
    /// </summary>
    /// <returns>The child window.</returns>
    IWindow CreateWindow();
}
