// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input;

/// <summary>
/// Represents an input device.
/// </summary>
public interface IInputDevice
{
    /// <summary>
    /// The device's name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Whether this device is connected or not.
    /// </summary>
    bool IsConnected { get; }
}
