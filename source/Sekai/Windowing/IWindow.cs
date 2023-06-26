// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Windowing;

/// <summary>
/// An interface for window objects.
/// </summary>
public interface IWindow : IDisposable
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
    /// Called when the window state has changed.
    /// </summary>
    event Action<WindowState>? StateChanged;

    /// <summary>
    /// Gets whether the window exists or not.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// The window's surface info.
    /// </summary>
    NativeWindowInfo Surface { get; }

    /// <summary>
    /// The monitor where this window is currently at.
    /// </summary>
    IMonitor Monitor { get; }

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
    Size Size { get; set; }

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
    /// The window's focus state.
    /// </summary>
    bool HasFocus { get; }

    /// <summary>
    /// The window's visibility state.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// The window's title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Translates screen-space coordinates to local coordinates.
    /// </summary>
    /// <param name="point">The local-space coordinates.</param>
    /// <returns>The screen-space coordinates.</returns>
    Point PointToClient(Point point);

    /// <summary>
    /// Translates local-space coordinates to screen coordinates.
    /// </summary>
    /// <param name="point">The local-space coordinates.</param>
    /// <returns>The screen-space coordinates.</returns>
    Point PointToScreen(Point point);

    /// <summary>
    /// Closes the window.
    /// </summary>
    void Close();

    /// <summary>
    /// Makes the window focused.
    /// </summary>
    void Focus();

    /// <summary>
    /// Performs window events.
    /// </summary>
    void DoEvents();
}
