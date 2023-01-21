// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Input.Devices.Touch;

namespace Sekai.Input.Events;

/// <summary>
/// Representsa touch move event.
/// </summary>
public sealed class TouchMoveEvent : TouchEvent
{
    /// <summary>
    /// The touch source's new position.
    /// </summary>
    public readonly Vector2 Position;

    public TouchMoveEvent(TouchSource source, Vector2 position)
        : base(source)
    {
        Position = position;
    }
}
