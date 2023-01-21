// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A gamepad/joystick trigger.
/// </summary>
public struct Trigger
{
    /// <summary>
    /// The trigger's index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// How far this trigger is pressed down.
    /// </summary>
    public float Position { get; }

    public Trigger(int index, float position)
    {
        Index = index;
        Position = position;
    }
}
