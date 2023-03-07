// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Surfaces;

/// <summary>
/// Window Border
/// </summary>
public enum WindowBorder
{
    /// <summary>
    /// The window can be resized by dragging its borders.
    /// </summary>
    Normal,

    /// <summary>
    /// The window border is visible but cannot be resized by the user.
    /// </summary>
    Fixed,

    /// <summary>
    /// The window border is hidden.
    /// </summary>
    Hidden,
}
