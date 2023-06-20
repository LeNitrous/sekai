// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Input;

/// <summary>
/// An enumeration of all joystick hat positions.
/// </summary>
[Flags]
public enum HatPosition
{
    /// <summary>
    /// Centered.
    /// </summary>
    Centered = 0,

    /// <summary>
    /// Up.
    /// </summary>
    Up = 1 << 0,

    /// <summary>
    /// Right.
    /// </summary>
    Right = 1 << 1,

    /// <summary>
    /// Down.
    /// </summary>
    Down = 1 << 2,

    /// <summary>
    /// Left.
    /// </summary>
    Left = 1 << 3,

    /// <summary>
    /// Up Left.
    /// </summary>
    UpLeft = Up | Left,

    /// <summary>
    /// Up Right.
    /// </summary>
    UpRight = Up | Right,

    /// <summary>
    /// Down Left.
    /// </summary>
    DownLeft = Down | Left,

    /// <summary>
    /// Down Right.
    /// </summary>
    DownRight = Down | Right,
}
