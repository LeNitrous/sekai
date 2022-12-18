// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input;

/// <summary>
/// Interface for generic input devices.
/// </summary>
public interface IInputDevice
{
    /// <summary>
    /// The name of the device.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The index of this device.
    /// </summary>
    int Index { get; }

    /// <summary>
    /// Whether this input device is currently connected.
    /// </summary>
    bool IsConnected { get; }
}
