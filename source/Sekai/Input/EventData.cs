// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Input;

[StructLayout(LayoutKind.Explicit)]
public struct EventData
{
    /// <summary>
    /// The source type.
    /// </summary>
    [FieldOffset(0)]
    public EventSource Source;

    /// <summary>
    /// The mouse scroll event data.
    /// </summary>
    [FieldOffset(4)]
    public MouseScrollEvent MouseScroll;

    /// <summary>
    /// The mouse button event data.
    /// </summary>
    [FieldOffset(4)]
    public MouseButtonEvent MouseButton;

    /// <summary>
    /// The mouse motion event data.
    /// </summary>
    [FieldOffset(4)]
    public MouseMotionEvent MouseMotion;

    /// <summary>
    /// The keyboard event data.
    /// </summary>
    [FieldOffset(4)]
    public KeyboardEvent Keyboard;
}

/// <summary>
/// Mouse motion event data.
/// </summary>
/// <param name="Position">The position of the mouse relative to screen coordinates.</param>
public readonly record struct MouseMotionEvent(Point Position);

/// <summary>
/// Mouse scroll event data.
/// </summary>
/// <param name="Delta">The scroll delta.</param>
public readonly record struct MouseScrollEvent(Vector2 Delta);

/// <summary>
/// Mouse button event data.
/// </summary>
/// <param name="Button">The affected button.</param>
/// <param name="Pressed">Whether the affected button is pressed or not.</param>
public readonly record struct MouseButtonEvent(MouseButton Button, bool Pressed);

/// <summary>
/// Keyboard event data.
/// </summary>
/// <param name="Key">The affected key.</param>
/// <param name="Pressed">Whether the affected button is pressed or not.</param>
public readonly record struct KeyboardEvent(Key Key, bool Pressed);
