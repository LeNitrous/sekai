// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework.Input;

namespace Sekai.Framework.Windowing;

public interface IView : IDisposable
{
    /// <summary>
    /// The active state of this view.
    /// </summary>
    bool Active { get; }

    /// <summary>
    /// Gets the input context for this window.
    /// </summary>
    IInputContext Input { get; }

    /// <summary>
    /// Called when the active state of this view changes.
    /// </summary>
    event Action<bool> OnStateChanged;

    /// <summary>
    /// Called when the view closes.
    /// </summary>
    event Action OnClose;

    /// <summary>
    /// Called when the view requests to be closed. Return true to continue closing.
    /// </summary>
    event Func<bool> OnCloseRequested;

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

    /// <summary>
    /// Close this view.
    /// </summary>
    void Close();
}
