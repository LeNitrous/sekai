// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Input;

/// <summary>
/// An interface representing a mouse.
/// </summary>
public interface IMouse : IPointer
{
    /// <summary>
    /// List of all buttons available buttons for this mouse.
    /// </summary>
    IReadOnlyList<MouseButton> SupportedButtons { get; }

    /// <summary>
    /// List of all scroll wheels available for this mouse.
    /// </summary>
    IReadOnlyList<ScrollWheel> ScrollWheels { get; }

    /// <summary>
    /// Called when a mouse button has been pressed down.
    /// </summary>
    event Action<IMouse, MouseButton> OnMouseDown;

    /// <summary>
    /// Called when a mouse button has been released.
    /// </summary>
    event Action<IMouse, MouseButton> OnMouseUp;

    /// <summary>
    /// Called when the mouse wheel scrolls.
    /// </summary>
    event Action<IMouse, ScrollWheel> OnScroll;

    /// <summary>
    /// Returns whether a given mouse button is pressed.
    /// </summary>
    bool IsButtonPressed(MouseButton button);

}
