// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Drawing;

namespace Sekai.Platform.Windowing;

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
    /// Gets the position of this monitor.
    /// </summary>
    Point Position { get; }

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
