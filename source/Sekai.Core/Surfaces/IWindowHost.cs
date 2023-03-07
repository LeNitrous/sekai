// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces;

/// <summary>
/// An interface for objects that can create and host windows.
/// </summary>
public interface IWindowHost
{
    /// <summary>
    /// Creates a window on this host.
    /// </summary>
    /// <returns>The createdw indow.</returns>
    IWindow CreateWindow();
}
