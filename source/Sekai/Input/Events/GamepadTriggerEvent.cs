// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Controllers;

namespace Sekai.Input.Events;

public class GamepadTriggerEvent : GamepadEvent
{
    /// <summary>
    /// The affected trigger.
    /// </summary>
    public readonly Trigger Trigger;

    public GamepadTriggerEvent(int index, Trigger trigger)
        : base(index)
    {
        Trigger = trigger;
    }
}
