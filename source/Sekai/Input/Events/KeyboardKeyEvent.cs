// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Keyboards;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a keyboard key event.
/// </summary>
public class KeyboardKeyEvent : KeyboardEvent
{
    /// <summary>
    /// The keyboard key.
    /// </summary>
    public readonly Key Key;

    /// <summary>
    /// The key's state.
    /// </summary>
    public readonly bool IsPressed;

    public KeyboardKeyEvent(Key key, bool isPressed)
    {
        Key = key;
        IsPressed = isPressed;
    }
}
