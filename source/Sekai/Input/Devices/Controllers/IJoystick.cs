// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A device that represents a joystick controller.
/// </summary>
public interface IJoystick : IController
{
    /// <summary>
    /// A list of all available axes.
    /// </summary>
    IReadOnlyList<Axis> Axes { get; }

    /// <summary>
    /// A list of all available hats.
    /// </summary>
    IReadOnlyList<Hat> Hats { get; }

    /// <summary>
    /// Called when a joystick axis is moved.
    /// </summary>
    event Action<IJoystick, Axis>? OnAxisMove;

    /// <summary>
    /// Called when a joystick hat is moved.
    /// </summary>
    event Action<IJoystick, Hat>? OnHatMove;
}
