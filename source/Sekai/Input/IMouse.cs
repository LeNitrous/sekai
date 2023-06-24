// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sekai.Input;

/// <summary>
/// Represents a mouse.
/// </summary>
public interface IMouse : IPointer
{
    /// <summary>
    /// The available buttons the mouse has.
    /// </summary>
    IReadOnlyList<MouseButton> Buttons { get; }

    /// <summary>
    /// The available scroll wheels the mouse has.
    /// </summary>
    IReadOnlyList<ScrollWheel> ScrollWheels { get; }

    /// <summary>
    /// Gets or sets the position of the mouse.
    /// </summary>
    new Vector2 Position { get; set; }

    /// <summary>
    /// Called when the mouse has scrolled.
    /// </summary>
    event Action<IMouse, ScrollWheel>? OnScroll;

    /// <summary>
    /// Called when a mouse button event has occured.
    /// </summary>
    event Action<IMouse, MouseButton, bool>? OnButton;
}
