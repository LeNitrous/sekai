// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input;

/// <summary>
/// Represents a controller.
/// </summary>
public interface IController : IInputDevice
{
    /// <summary>
    /// Gets the controller index.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// The controller's deadzone method.
    /// </summary>
    Deadzone Deadzone { get; set; }

    /// <summary>
    /// The available buttons the controller has.
    /// </summary>
    IReadOnlyList<Button> Buttons { get; }

    /// <summary>
    /// Called when a controller button event has occured.
    /// </summary>
    event Action<IController, Button>? OnButton;
}
