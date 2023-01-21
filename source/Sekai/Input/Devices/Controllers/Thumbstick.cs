// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Input.Devices.Controllers;

/// <summary>
/// A gamepad thumbstick.
/// </summary>
public struct Thumbstick
{
    /// <summary>
    /// The thumbstick's index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The X-axis position of the stick, from -1.0 to 1.0.
    /// </summary>
    public float X { get; }

    /// <summary>
    /// The Y-axis position of the stick, from -1.0 to 1.0.
    /// </summary>
    public float Y { get; }

    /// <summary>
    /// The current position of the stick, from 0.0 to 1.0.
    /// </summary>
    public float Postiion => (float)Math.Sqrt(X * X + Y * Y);

    /// <summary>
    /// The current direction of the stick, from -π to π.
    /// </summary>
    public float Direction => (float)Math.Atan2(Y, X);

    public Thumbstick(int index, float x, float y)
    {
        Index = index;
        X = x;
        Y = y;
    }
}
