// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Framework.Windowing;

public interface IView
{
    /// <summary>
    /// Gets whether this view is currently focused.
    /// </summary>
    bool Focused { get; }

    /// <summary>
    /// The monitor this view is currently present on.
    /// </summary>
    IMonitor Monitor { get; }

    /// <summary>
    /// The video mode this view is currently using.
    /// </summary>
    VideoMode VideoMode { get; }

    /// <summary>
    /// Called when the view closes.
    /// </summary>
    event Action OnClose;

    /// <summary>
    /// Called when the view requests to be closed. Return true to continue closing.
    /// </summary>
    event Func<bool> OnCloseRequested;

    /// <summary>
    /// Called when the view focus changes.
    /// </summary>
    event Action<bool> OnFocusChanged;

    /// <summary>
    /// Convert this point to screen coordinates.
    /// </summary>
    Point PointToScreen(Point point);

    /// <summary>
    /// Convert this point to client coordinates.
    /// </summary>
    Point PointToClient(Point point);

    /// <summary>
    /// Process events for this view.
    /// </summary>
    void DoEvents();
}
