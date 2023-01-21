// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Pointers;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a pen button event.
/// </summary>
public sealed class PenButtonEvent : PenEvent
{
    /// <summary>
    /// The affected button.
    /// </summary>
    public readonly PenButton Button;

    /// <summary>
    /// The button's state.
    /// </summary>
    public readonly bool IsPressed;

    public PenButtonEvent(PenButton button, bool isPressed)
    {
        Button = button;
        IsPressed = isPressed;
    }
}
