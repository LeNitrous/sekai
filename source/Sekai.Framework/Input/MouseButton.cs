// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Input;

/// <summary>
/// Mouse buttons.
/// </summary>
[Flags]
public enum MouseButton : uint
{
    /// <summary>
    /// Left mouse button.
    /// </summary>
    Left = 1 << 0,

    /// <summary>
    /// Right mouse button.
    /// </summary>
    Right = 1 << 1,

    /// <summary>
    /// Middle mouse button.
    /// </summary>
    Middle = 1 << 2,

    /// <summary>
    /// Extra mouse button 1.
    /// </summary>
    Button1 = 1 << 3,

    /// <summary>
    /// Extra mouse button 2.
    /// </summary>
    Button2 = 1 << 4,

    /// <summary>
    /// Extra mouse button 3.
    /// </summary>
    Button3 = 1 << 5,

    /// <summary>
    /// Extra mouse button 4.
    /// </summary>
    Button4 = 1 << 6,

    /// <summary>
    /// Extra mouse button 5.
    /// </summary>
    Button5 = 1 << 7,

    /// <summary>
    /// Extra mouse button 6.
    /// </summary>
    Button6 = 1 << 8,

    /// <summary>
    /// Extra mouse button 7.
    /// </summary>
    Button7 = 1 << 9,

    /// <summary>
    /// Extra mouse button 8.
    /// </summary>
    Button8 = 1 << 10,

    /// <summary>
    /// Extra mouse button 9.
    /// </summary>
    Button9 = 1 << 11,

    /// <summary>
    /// Extra mouse button 10.
    /// </summary>
    Button10 = 1 << 12,

    /// <summary>
    /// Extra mouse button 11.
    /// </summary>
    Button11 = 1 << 13,

    /// <summary>
    /// Extra mouse button 12.
    /// </summary>
    Button12 = 1 << 14,
}
