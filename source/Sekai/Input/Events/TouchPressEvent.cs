// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Touch;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a touch down or up event.
/// </summary>
public sealed class TouchPressEvent : TouchEvent
{
    /// <summary>
    /// Gets whether the touch is pressed or not.
    /// </summary>
    public readonly bool IsPressed;

    public TouchPressEvent(TouchSource source, bool isPressed)
        : base(source)
    {
        IsPressed = isPressed;
    }
}
