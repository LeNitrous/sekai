// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Input;

/// <summary>
/// An interface representing pointing devices.
/// </summary>
public interface IPointer : IInputDevice
{
    /// <summary>
    /// Represents the current position of the pointing device.
    /// </summary>
    Vector2 Position { get; set; }

    /// <summary>
    /// Called when the position of the pointing device changes.
    /// </summary>
    event Action<IPointer, Vector2> OnMove;
}
