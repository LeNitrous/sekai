// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Windowing;

/// <summary>
/// A view.
/// </summary>
public interface IView : IDisposable
{
    /// <summary>
    /// The view's size.
    /// </summary>
    Size Size { get; }

    /// <summary>
    /// Called every frame.
    /// </summary>
    event Action? Tick;

    /// <summary>
    /// Called when the view has been resized.
    /// </summary>
    event Action<Size>? Resized;

    /// <summary>
    /// Called when the view has resumed to foreground.
    /// </summary>
    event Action? Resumed;

    /// <summary>
    /// Called when the view has suspended into background.
    /// </summary>
    event Action? Suspend;

    /// <summary>
    /// Called when the view has been closed.
    /// </summary>
    event Action? Closed;

    /// <summary>
    /// Called when the view is about to close.
    /// </summary>
    event Action? Closing;

    /// <summary>
    /// Called when the view is requesting to be closed. Return <see langword="true"/> to continue closing.
    /// </summary>
    event Func<bool>? CloseRequested;

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
}
