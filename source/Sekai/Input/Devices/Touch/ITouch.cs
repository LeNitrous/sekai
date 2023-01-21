// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Input.Devices.Touch;

/// <summary>
/// Represents a device which is a touch-capable device.
/// </summary>
public interface ITouch : IInputDevice
{
    /// <summary>
    /// Called when a touch has moved.
    /// </summary>
    event Action<ITouch, TouchData>? OnMove;

    /// <summary>
    /// Called when a touch has been held down.
    /// </summary>
    event Action<ITouch, TouchData>? OnPressed;

    /// <summary>
    /// Called when a touch has been released.
    /// </summary>
    event Action<ITouch, TouchData>? OnRelease;
}
