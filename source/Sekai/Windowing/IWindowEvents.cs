// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Windowing;

public interface IWindowEvents
{
    /// <summary>
    /// Called when the window resizes.
    /// </summary>
    event Action<Size2>? OnResize;

    /// <summary>
    /// Called when the window's position is changed.
    /// </summary>
    event Action<Point>? OnMoved;

    /// <summary>
    /// Called when the window focus changes.
    /// </summary>
    event Action<bool>? OnFocusChanged;

    /// <summary>
    /// Called when a file has been dropped to this window.
    /// </summary>
    event Action<string[]>? OnDataDropped;
}
