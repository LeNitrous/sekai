// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A rumble motor inside a gamepad.
/// </summary>
public interface IMotor
{
    /// <summary>
    /// The motor's index.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// The motor's vibration intensity, between 0.0 to 1.0.
    /// </summary>
    float Speed { get; set; }
}
