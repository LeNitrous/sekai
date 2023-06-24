// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input;

/// <summary>
/// Represents a gamepad.
/// </summary>
public interface IGamepad : IController
{
    /// <summary>
    /// The available thumbsticks the gamepad has.
    /// </summary>
    IReadOnlyList<Thumbstick> Thumbsticks { get; }

    /// <summary>
    /// The available triggers the gamepad has.
    /// </summary>
    IReadOnlyList<Trigger> Triggers { get; }

    /// <summary>
    /// The available rumble motors the gamepad has.
    /// </summary>
    IReadOnlyList<IMotor> Motors { get; }

    /// <summary>
    /// Called when the gamepad trigger has moved.
    /// </summary>
    event Action<IGamepad, Trigger>? OnTriggerMove;

    /// <summary>
    /// Called when the gamepad thumbstick has moved.
    /// </summary>
    event Action<IGamepad, Thumbstick>? OnThumbstickMove;
}
