// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Drawing;

namespace Sekai.Platform;

/// <summary>
/// An interface for objects that represent a physical monitor.
/// </summary>
public interface IMonitor
{
    /// <summary>
    /// Gets the name of this monitor.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the index of this monitor.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Gets the bounds of this monitor.
    /// </summary>
    Rectangle Bounds { get; }

    /// <summary>
    /// Gets the current video mode of this monitor.
    /// </summary>
    VideoMode Mode { get; }

    /// <summary>
    /// Gets all video modes that this monitor supports.
    /// </summary>
    /// <returns>An enumeration of supported video modes.</returns>
    IEnumerable<VideoMode> GetSupportedVideoModes();
}
