// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// Represents the host's platform.
/// </summary>
public abstract class Platform : IDisposable
{
    /// <summary>
    /// Gets the primary monitor.
    /// </summary>
    public abstract IMonitor PrimaryMonitor { get; }

    /// <summary>
    /// Gets all of the available monitors.
    /// </summary>
    public abstract IEnumerable<IMonitor> Monitors { get; }

    /// <summary>
    /// Creates a window.
    /// </summary>
    public abstract IWindow CreateWindow();

    public abstract void Dispose();
}
