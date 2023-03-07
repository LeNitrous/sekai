// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Surfaces;

/// <summary>
/// An interface for objects that handle windowed surfaces.
/// </summary>
public interface IWindow : IView
{
    /// <summary>
    /// Gets the host which created this window.
    /// </summary>
    IWindowHost? Parent { get; }

    /// <summary>
    /// Gets the monitor the window is present on.
    /// </summary>
    IMonitor? Monitor { get; }

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets or sets the icon for this window.
    /// </summary>
    Icon Icon { get; set; }

    /// <summary>
    /// Gets or sets the position of this window.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// Gets or sets the size of this window.
    /// </summary>
    new Size Size { get; set; }

    /// <summary>
    /// Gets or sets the minimum size of this window.
    /// </summary>
    Size MinimumSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of this window.
    /// </summary>
    Size MaximumSize { get; set; }

    /// <summary>
    /// Gets or sets the state of this window.
    /// </summary>
    WindowState State { get; set; }

    /// <summary>
    /// Gets or sets the borders of this window.
    /// </summary>
    WindowBorder Border { get; set; }

    /// <summary>
    /// Raised when the window has been resized.
    /// </summary>
    event Action<Size>? Resized;

    /// <summary>
    /// Raised when the window has been moved.
    /// </summary>
    event Action<Point>? Moved;

    /// <summary>
    /// Raised when the user drops content onto the window.
    /// </summary>
    event Action<string[]>? Dropped;

    /// <summary>
    /// Raised when the window state changes.
    /// </summary>
    event Action<WindowState>? StateChanged;
}
