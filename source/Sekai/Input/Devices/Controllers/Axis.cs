// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A joystick axis.
/// </summary>
public readonly struct Axis
{
    /// <summary>
    /// The axis index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The axis position
    /// </summary>
    public float Position { get; }

    public Axis(int index, float position)
    {
        Index = index;
        Position = position;
    }
}
