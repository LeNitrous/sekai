// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Controllers;

namespace Sekai.Input.Events;

public class JoystickAxisEvent : JoystickEvent
{
    /// <summary>
    /// The affected joystick axis.
    /// </summary>
    public readonly Axis Axis;

    public JoystickAxisEvent(int index, Axis axis)
        : base(index)
    {
        Axis = axis;
    }
}
