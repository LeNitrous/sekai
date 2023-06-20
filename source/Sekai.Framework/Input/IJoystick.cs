// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a joystick.
/// </summary>
public interface IJoystick : IController
{
    /// <summary>
    /// The available axes the joystick has.
    /// </summary>
    IReadOnlyList<Axis> Axes { get; }

    /// <summary>
    /// The available hats the joystick has.
    /// </summary>
    IReadOnlyList<Hat> Hats { get; }

    /// <summary>
    /// Called when the joystick hat has been moved.
    /// </summary>
    event Action<IJoystick, Hat>? OnHatMove;

    /// <summary>
    /// Called when the joystick axis has been moved.
    /// </summary>
    event Action<IJoystick, Axis>? OnAxisMove;
}
