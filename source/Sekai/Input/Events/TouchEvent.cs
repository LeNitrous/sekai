// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.Touch;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a touch event.
/// </summary>
public abstract class TouchEvent : DeviceEvent
{
    /// <summary>
    /// The touch source.
    /// </summary>
    public readonly TouchSource Source;

    protected TouchEvent(TouchSource source)
    {
        Source = source;
    }
}
