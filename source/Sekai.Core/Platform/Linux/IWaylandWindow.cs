// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Platform.Linux;

/// <summary>
/// Am interface for Wayland windows.
/// </summary>
public interface IWaylandWindow
{
    /// <summary>
    /// Gets whether the windowing system uses Wayland.
    /// </summary>
    bool IsWayland { get; }

    /// <summary>
    /// The wayland display handle.
    /// </summary>
    nint Display { get; }

    /// <summary>
    /// The wayland surface handle.
    /// </summary>
    nint Surface { get; }
}
