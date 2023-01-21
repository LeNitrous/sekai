// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input;

public abstract class InputSystem : DependencyObject
{
    /// <summary>
    /// A list of all available input devices.
    /// </summary>
    public abstract IReadOnlyList<IInputDevice> Connected { get; }

    /// <summary>
    /// Called when the connection status of a device changes.
    /// </summary>
    public abstract event Action<IInputDevice, bool>? OnConnectionChanged;
}
