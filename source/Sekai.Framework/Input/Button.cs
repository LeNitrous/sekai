// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a controller button.
/// </summary>
public readonly struct Button : IEquatable<Button>
{
    /// <summary>
    /// The button's name.
    /// </summary>
    /// <remarks>
    /// This value is guaranteed to be valid if it comes from a gamepad.
    /// </remarks>
    public ButtonName Name { get; }

    /// <summary>
    /// The button's index.
    /// </summary>
    /// <remarks>
    /// This value is guaranteed to be valid if it comes from a joystick.
    /// </remarks>
    public int Index { get; }

    /// <summary>
    /// Whether this button is pressed or not.
    /// </summary>
    public bool Pressed { get; }

    /// <summary>
    /// Creates a button using a <see cref="ButtonName"/>.
    /// </summary>
    /// <param name="index">The button's index.</param>
    /// <param name="name">The button's name.</param>
    /// <param name="pressed">Whether this button is pressed or not.</param>
    public Button(int index, ButtonName name, bool pressed)
    {
        Name = name;
        Index = index;
        Pressed = pressed;
    }

    /// <summary>
    /// Creates a button using an index.
    /// </summary>
    /// <param name="index">The button's index.</param>
    /// <param name="pressed">Whether this button is pressed or not.</param>
    public Button(int index, bool pressed)
    {
        Name = ButtonName.Unknown;
        Index = index;
        Pressed = pressed;
    }

    public bool Equals(Button other)
    {
        return Name == other.Name && Index.Equals(other.Index) && Pressed.Equals(other.Pressed);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is Button button && Equals(button);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Index, Pressed);
    }

    public static bool operator ==(Button left, Button right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Button left, Button right)
    {
        return !(left == right);
    }
}
