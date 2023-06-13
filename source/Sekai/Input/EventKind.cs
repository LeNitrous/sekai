// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input;

/// <summary>
/// The kind of event that occured.
/// </summary>
public enum EventKind
{
    /// <summary>
    /// Unknown.
    /// </summary>
    Unknown,

    /// <summary>
    /// Mouse motion event.
    /// </summary>
    MouseMotion,

    /// <summary>
    /// Mouse scroll event.
    /// </summary>
    MouseScroll,

    /// <summary>
    /// Mouse button event.
    /// </summary>
    MouseButton,

    /// <summary>
    /// Keyboard event.
    /// </summary>
    Keyboard,
}
