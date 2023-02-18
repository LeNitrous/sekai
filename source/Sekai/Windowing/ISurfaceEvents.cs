// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Windowing;

public interface ISurfaceEvents
{
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
}
