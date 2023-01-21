// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Controllers;

namespace Sekai.Input.Events;

public class ControllerButtonEvent : JoystickEvent
{
    /// <summary>
    /// The affected button.
    /// </summary>
    public readonly ControllerButton Button;

    /// <summary>
    /// The button state.
    /// </summary>
    public readonly bool IsPressed;

    public ControllerButtonEvent(int index, ControllerButton button, bool isPressed)
        : base(index)
    {
        Button = button;
        IsPressed = isPressed;
    }
}
