// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System;

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A device which represents a game controller.
/// </summary>
public interface IController : IInputDevice
{
    /// <summary>
    /// The joystick deadzone.
    /// </summary>
    Deadzone Deadzone { get; set; }

    /// <summary>
    /// A collection of <see cref="ControllerButton"/>s this controller supports.
    /// </summary>
    IReadOnlyList<ControllerButton> Buttons { get; }

    /// <summary>
    /// Called when a <see cref="ControllerButton"/> has been pressed down.
    /// </summary>
    event Action<IController, ControllerButton>? OnButtonPressed;

    /// <summary>
    /// Called when a <see cref="ControllerButton"/> has been released.
    /// </summary>
    event Action<IController, ControllerButton>? OnButtonRelease;
}
