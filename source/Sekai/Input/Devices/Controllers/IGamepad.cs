// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A device that represents a gamepad.
/// </summary>
public interface IGamepad : IController
{
    /// <summary>
    /// A list of all available thumbsticks.
    /// </summary>
    IReadOnlyList<Thumbstick> Thumbsticks { get; }

    /// <summary>
    /// A list of all available triggers.
    /// </summary>
    IReadOnlyList<Trigger> Triggers { get; }

    /// <summary>
    /// A list of all vibration motors.
    /// </summary>
    IReadOnlyList<IMotor> Motors { get; }

    /// <summary>
    /// Called when a thumbstick is moved.
    /// </summary>
    event Action<IGamepad, Thumbstick>? OnThumbstickMove;

    /// <summary>
    /// Called when a trigger is moved.
    /// </summary>
    event Action<IGamepad, Trigger>? OnTriggerMove;
}
