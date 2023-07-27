// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Windowing;

/// <summary>
/// Represents a display monitor.
/// </summary>
public abstract class Monitor : IWindowHost
{
    /// <summary>
    /// The index of this monitor.
    /// </summary>
    public abstract int Index { get; }

    /// <summary>
    /// The monitor's display name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets whether this monitor is primary or not.
    /// </summary>
    public abstract bool Primary { get; }

    /// <summary>
    /// The monitor's current video mode.
    /// </summary>
    public abstract VideoMode Mode { get; }

    public abstract IWindow CreateWindow();

    /// <summary>
    /// Gets all available video modes this monitor can support.
    /// </summary>
    public abstract IEnumerable<VideoMode> GetVideoModes();
}
