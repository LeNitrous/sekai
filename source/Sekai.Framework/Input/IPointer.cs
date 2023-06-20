// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Framework.Input;

/// <summary>
/// Represents a pointing device.
/// </summary>
public interface IPointer : IInputDevice
{
    /// <summary>
    /// The position of the pointer.
    /// </summary>
    Vector2 Position { get; }

    /// <summary>
    /// Called when the pointer has changed position.
    /// </summary>
    event Action<IPointer, Vector2>? OnMove;
}
