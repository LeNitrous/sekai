// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Surfaces;

/// <summary>
/// An interface for objects that handle fullscreen surfaces.
/// </summary>
public interface IView : ISurface
{
    /// <summary>
    /// Gets whether this view has focus.
    /// </summary>
    bool Focus { get; }

    /// <summary>
    /// Raised when the view is requested to close.
    /// </summary>
    event Func<bool>? CloseRequested;

    /// <summary>
    /// Raised when the view has closed.
    /// </summary>
    event Action? Closed;

    /// <summary>
    /// Raised when the view's focus changes.
    /// </summary>
    event Action<bool>? FocusChanged;

    /// <summary>
    /// Processes view's events.
    /// </summary>
    void DoEvents();

    /// <summary>
    /// Closes this view.
    /// </summary>
    void Close();
}
