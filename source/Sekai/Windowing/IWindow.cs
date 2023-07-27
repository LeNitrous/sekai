// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Mathematics;

namespace Sekai.Windowing;

/// <summary>
/// A window.
/// </summary>
public interface IWindow : IView, IWindowHost
{
    /// <summary>
    /// Gets whether the window has input focus or not.
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
    /// The window's size.
    /// </summary>
    new Size Size { get; set; }

    /// <summary>
    /// The monitor this window is present on.
    /// </summary>
    Monitor Monitor { get; }

    /// <summary>
    /// Gets all the monitors currently available.
    /// </summary>
    IEnumerable<Monitor> Monitors { get; }

    /// <summary>
    /// The host owning this window.
    /// </summary>
    IWindowHost Owner { get; }

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
    /// The window state.
    /// </summary>
    WindowState State { get; set; }

    /// <summary>
    /// The window border state.
    /// </summary>
    WindowBorder Border { get; set; }

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
    /// Called when content has been dropped on the window.
    /// </summary>
    event Action<string[]>? Dropped;

    /// <summary>
    /// Makes the window focused.
    /// </summary>
    void Focus();

    /// <summary>
    /// Runs the window loop on the calling thread.
    /// </summary>
    void Run();

    /// <summary>
    /// Sets the window's icons.
    /// </summary>
    /// <param name="icons">The icons to use.</param>
    void SetWindowIcon(ReadOnlySpan<Icon> icons);
}
