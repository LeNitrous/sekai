// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a mouse move event.
/// </summary>
public class MouseMoveEvent : MouseEvent
{
    /// <summary>
    /// The mouse's new position.
    /// </summary>
    public readonly Vector2 Position;

    public MouseMoveEvent(Vector2 position)
    {
        Position = position;
    }
}
