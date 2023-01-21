// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

/// <summary>
/// Represents a controller event.
/// </summary>
public abstract class ControllerEvent : DeviceEvent
{
    /// <summary>
    /// The controller index.
    /// </summary>
    public readonly int Index;

    protected ControllerEvent(int index)
    {
        Index = index;
    }
}
