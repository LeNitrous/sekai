// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Mathematics;

namespace Sekai.Windowing;

public interface IWindow : ISurface
{
    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets whether this window is currently focused.
    /// </summary>
    bool Focused { get; }

    /// <summary>
    /// Gets or sets the window icon.
    /// </summary>
    Icon Icon { get; set; }

    /// <summary>
    /// Gets or sets the position of the window.
    /// </summary>
    new Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of the window.
    /// </summary>
    new Size2 Size { get; set; }

    /// <summary>
    /// Gets or sets the minimum size of this window.
    /// </summary>
    Size2 MinimumSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of this window.
    /// </summary>
    Size2 MaximumSize { get; set; }

    /// <summary>
    /// Gets or sets the state of this window.
    /// </summary>
    WindowState State { get; set; }

    /// <summary>
    /// Gets or sets the window borders.
    /// </summary>
    WindowBorder Border { get; set; }

    /// <summary>
    /// Gets the current monitor this window is on.
    /// </summary>
    IMonitor Monitor { get; }

    /// <summary>
    /// Gets all the monitors on this system.
    /// </summary>
    IEnumerable<IMonitor> Monitors { get; }

    /// <summary>
    /// Gets or sets whether this window is visible.
    /// </summary>
    bool Visible { get; set; }
}
