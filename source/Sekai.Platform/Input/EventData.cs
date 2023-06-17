// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Sekai.Platform.Input;

[StructLayout(LayoutKind.Explicit)]
public readonly struct EventData
{
    [FieldOffset(0)]
    public readonly EventKind Kind;

    [FieldOffset(4)]
    public readonly MouseScrollEvent MouseScroll;

    [FieldOffset(4)]
    public readonly MouseButtonEvent MouseButton;

    [FieldOffset(4)]
    public readonly MouseMotionEvent MouseMotion;

    [FieldOffset(4)]
    public readonly KeyboardEvent Keyboard;

    [FieldOffset(4)]
    public readonly ControllerHatEvent ControllerHat;

    [FieldOffset(4)]
    public readonly ControllerAxisEvent ControllerAxis;

    [FieldOffset(4)]
    public readonly ControllerButtonEvent ControllerButton;

    public EventData(MouseScrollEvent scroll)
    {
        Kind = EventKind.MouseScroll;
        MouseScroll = scroll;
    }

    public EventData(MouseButtonEvent button)
    {
        Kind = EventKind.MouseButton;
        MouseButton = button;
    }

    public EventData(MouseMotionEvent motion)
    {
        Kind = EventKind.MouseMotion;
        MouseMotion = motion;
    }

    public EventData(KeyboardEvent keyboard)
    {
        Kind = EventKind.Keyboard;
        Keyboard = keyboard;
    }

    public EventData(ControllerHatEvent hat)
    {
        Kind = EventKind.ControllerHat;
        ControllerHat = hat;
    }

    public EventData(ControllerAxisEvent axis)
    {
        Kind = EventKind.ControllerAxis;
        ControllerAxis = axis;
    }

    public EventData(ControllerButtonEvent button)
    {
        Kind = EventKind.ControllerButton;
        ControllerButton = button;
    }
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

/// <summary>
/// Controller axis event data.
/// </summary>
/// <param name="Controller">The controller index that triggered the event.</param>
/// <param name="Axis">The index of the affected axis.</param>
/// <param name="Value">The new axis value.</param>
public readonly record struct ControllerAxisEvent(int Controller, int Axis, float Value);

/// <summary>
/// Controller button event data.
/// </summary>
/// <param name="Controller">The controller index that triggered the event.</param>
/// <param name="Button">The affected button.</param>
/// <param name="Pressed">Whether the affected button is pressed or not.</param>
public readonly record struct ControllerButtonEvent(int Controller, ControllerButton Button, bool Pressed);

/// <summary>
/// Joystick hat event data.
/// </summary>
/// <param name="Joystick">The controller index that triggered the event.</param>
/// <param name="Hat">The index of the affected hat.</param>
/// <param name="Position">The new hat position.</param>
public readonly record struct ControllerHatEvent(int Joystick, int Hat, HatPosition Position);
