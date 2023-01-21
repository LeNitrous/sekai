// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// Represents a joystick hat.
/// </summary>
public struct Hat
{
    /// <summary>
    /// The joystick hat index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The joystick hat position.
    /// </summary>
    public HatPosition Position { get; }

    public Hat(int index, HatPosition position)
    {
        Index = index;
        Position = position;
    }
}
