// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input;

/// <summary>
/// The input source hosting all available devices.
/// </summary>
public interface IInputSource
{
    /// <summary>
    /// Gets all connected devices from this source.
    /// </summary>
    IEnumerable<IInputDevice> Devices { get; }

    /// <summary>
    /// Called when a device's connection has been changed.
    /// </summary>
    event Action<IInputDevice, bool>? ConnectionChanged;
}
