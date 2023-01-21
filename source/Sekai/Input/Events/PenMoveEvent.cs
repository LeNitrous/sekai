// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a pen move event.
/// </summary>
public sealed class PenMoveEvent : PenEvent
{
    /// <summary>
    /// The new pen position.
    /// </summary>
    public readonly Vector2 Position;

    public PenMoveEvent(Vector2 position)
    {
        Position = position;
    }
}
