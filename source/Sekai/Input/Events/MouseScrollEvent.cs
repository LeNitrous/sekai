// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Pointers;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a mouse scroll event.
/// </summary>
public class MouseScrollEvent : MouseEvent
{
    /// <summary>
    /// The mouse's current scroll wheel.
    /// </summary>
    public readonly ScrollWheel Scroll;

    public MouseScrollEvent(ScrollWheel scroll)
    {
        Scroll = scroll;
    }
}
