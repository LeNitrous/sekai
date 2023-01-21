// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Controllers;

namespace Sekai.Input.Events;

public class JoystickHatEvent : JoystickEvent
{
    /// <summary>
    /// The affected joystick hat.
    /// </summary>
    public readonly Hat Hat;

    public JoystickHatEvent(int index, Hat hat)
        : base(index)
    {
        Hat = hat;
    }
}
