// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Windowing;

public interface ISurface : IDisposable
{
    /// <summary>
    /// The active state of this view.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Gets or sets the size of the view.
    /// </summary>
    Size2 Size { get; }

    /// <summary>
    /// Gets or sets the position of the window.
    /// </summary>
    Point Position { get; }

    /// <summary>
    /// Called when the active state of this view changes.
    /// </summary>
    event Action<bool>? OnStateChanged;

    /// <summary>
    /// Called when the view closes.
    /// </summary>
    event Action? OnClose;

    /// <summary>
    /// Called when the view requests to be closed. Return true to continue closing.
    /// </summary>
    event Func<bool>? OnCloseRequested;

    /// <summary>
    /// Convert this point to screen coordinates.
    /// </summary>
    Point PointToScreen(Point point);

    /// <summary>
    /// Convert this point to client coordinates.
    /// </summary>
    Point PointToClient(Point point);

    /// <summary>
    /// Runs the window loop.
    /// </summary>
    void Run();

    /// <summary>
    /// Close this view.
    /// </summary>
    void Close();
}
