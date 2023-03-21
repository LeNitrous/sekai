// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Platform;

/// <summary>
/// An interface for window objects.
/// </summary>
public interface IWindow : ISurface, IDisposable
{
    /// <summary>
    /// Called when the window has been closed.
    /// </summary>
    event Action? Closed;

    /// <summary>
    /// Called when the window is about to close.
    /// </summary>
    event Action? Closing;

    /// <summary>
    /// Called when the window is requesting to be closed. Return <see langword="true"/> to continue closing.
    /// </summary>
    event Func<bool>? CloseRequested;

    /// <summary>
    /// Called when the window has been resized.
    /// </summary>
    event Action<Size>? Resized;

    /// <summary>
    /// Called when the window has been moved.
    /// </summary>
    event Action<Point>? Moved;

    /// <summary>
    /// Called when the window focus has changed.
    /// </summary>
    event Action<bool>? FocusChanged;

    /// <summary>
    /// The window that created this child window.
    /// </summary>
    IWindow? Parent { get; }

    /// <summary>
    /// The monitor where this window is currently at.
    /// </summary>
    IMonitor? Monitor { get; }

    /// <summary>
    /// The window border state.
    /// </summary>
    WindowBorder Border { get; set; }

    /// <summary>
    /// The window state.
    /// </summary>
    WindowState State { get; set; }

    /// <summary>
    /// The window's size.
    /// </summary>
    new Size Size { get; set; }

    /// <summary>
    /// The window's minimum size.
    /// </summary>
    Size MinimumSize { get; set; }

    /// <summary>
    /// The window's maximum size.
    /// </summary>
    Size MaximumSize { get; set; }

    /// <summary>
    /// The window's position.
    /// </summary>
    Point Position { get; set; }

    /// <summary>
    /// The window's focus.
    /// </summary>
    bool Focus { get; }

    /// <summary>
    /// Gets whether the window exists or not.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Gets whether th window is visible or not.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// The window's title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Creates a new child window.
    /// </summary>
    /// <returns>The child window.</returns>
    IWindow CreateWindow();

    /// <summary>
    /// Closes the window.
    /// </summary>
    void Close();
}
