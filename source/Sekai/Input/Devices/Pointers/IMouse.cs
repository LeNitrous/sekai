// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Input.Devices.Pointers;

/// <summary>
/// An interface representing a mouse.
/// </summary>
public interface IMouse : IPointer
{
    /// <summary>
    /// A collection of buttons the mouse supports.
    /// </summary>
    IReadOnlyList<MouseButton> Buttons { get; }

    /// <summary>
    /// A collection of al scroll wheels this mouse has.
    /// </summary>
    IReadOnlyList<ScrollWheel> ScrollWheels { get; }

    /// <summary>
    /// Called when a <see cref="MouseButton"/> has been pressed down.
    /// </summary>
    event Action<IMouse, MouseButton>? OnButtonPressed;

    /// <summary>
    /// Called when a <see cref="MouseButton"/> has been released.
    /// </summary>
    event Action<IMouse, MouseButton>? OnButtonRelease;

    /// <summary>
    /// Called when the mouse wheel scrolls.
    /// </summary>
    event Action<IMouse, ScrollWheel>? OnScroll;
}
