// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Controllers;

namespace Sekai.Input.Events;

public class GamepadThumbstickEvent : GamepadEvent
{
    /// <summary>
    /// The affected thumbstick.
    /// </summary>
    public readonly Thumbstick Thumbstick;

    public GamepadThumbstickEvent(int index, Thumbstick thumbstick)
        : base(index)
    {
        Thumbstick = thumbstick;
    }
}
