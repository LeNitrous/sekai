// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input;

/// <summary>
/// The system responsible for interacting with low level input related functionality.
/// </summary>
public abstract class InputSystem : DisposableObject
{
    /// <summary>
    /// The input system name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// The input system version.
    /// </summary>
    public abstract Version Version { get; }

    /// <summary>
    /// A list of all available input devices.
    /// </summary>
    public abstract IReadOnlyList<IInputDevice> Connected { get; }

    /// <summary>
    /// Called when the connection status of a device changes.
    /// </summary>
    public abstract event Action<IInputDevice, bool>? OnConnectionChanged;
}
