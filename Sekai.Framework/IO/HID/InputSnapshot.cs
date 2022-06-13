// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Silk.NET.SDL;
using System.Collections.Generic;
using System.Numerics;

namespace Sekai.Framework.IO.HID;

/// <summary>
/// Snapshot of a input frame
/// </summary>
public struct InputSnapshot
{
    /// <summary>
    /// List of keyboard events of this frame.
    /// </summary>
    public IReadOnlyList<KeyboardEvent> KeyboardEvents { get; }

    /// <summary>
    /// List of mouse events of this frame.
    /// </summary>
    public IReadOnlyList<MouseEvent> MouseEvents { get; }

    /// <summary>
    /// List of joystick events of this frame.
    /// </summary>
    public IReadOnlyList<JoystickEvent> JoystickEvents { get; }

    /// <summary>
    /// List of key characters were pressed during this frame.
    /// </summary>
    public IReadOnlyList<int> KeyPresses { get; }

    /// <summary>
    /// Checks if the mouse is down on this frame.
    /// </summary>
    public bool IsMouseDown { get; }

    /// <summary>
    /// The position of the mouse during this frame.
    /// </summary>
    public Vector2 MousePos { get; }

    /// <summary>
    /// The mouse wheel's delta during this frame.
    /// </summary>
    public float MouseWheelDelta { get; }
}


public struct MouseEvent
{

    public IReadOnlyList<MouseButtonEvent> MouseButtonEvents { get; }

    public IReadOnlyList<MouseWheelEvent> MouseWheelEvents { get; }

    public IReadOnlyList<MouseMotionEvent> MouseMotionEvents { get; }
}

public struct JoystickEvent
{
    public IReadOnlyList<JoyButtonEvent> JoyButtonEvents { get; }

    public IReadOnlyList<JoyHatEvent> JoyHatEvents { get; }

    public IReadOnlyList<JoyBallEvent> JoyBallEvents { get; }

    public IReadOnlyList<JoyAxisEvent> JoyAxisEvents { get; }
}
