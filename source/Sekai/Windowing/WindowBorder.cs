// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Windowing;

public enum WindowBorder
{
    /// <summary>
    /// The window can be resized by dragging its borders.
    /// </summary>
    Resizable,

    /// <summary>
    /// The window border is visible but cannot be resized by the user.
    /// </summary>
    Fixed,

    /// <summary>
    /// The window border is hidden.
    /// </summary>
    Hidden,
}
