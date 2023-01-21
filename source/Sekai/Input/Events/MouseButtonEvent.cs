// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Pointers;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a mouse button event.
/// </summary>
public class MouseButtonEvent : MouseEvent
{
    /// <summary>
    /// The mouse button.
    /// </summary>
    public readonly MouseButton Button;

    /// <summary>
    /// The mouse button's state.
    /// </summary>
    public readonly bool IsPressed;

    public MouseButtonEvent(MouseButton button, bool isPressed)
    {
        Button = button;
        IsPressed = isPressed;
    }
}
