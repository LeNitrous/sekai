// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

/// <summary>
/// Represents a controller connection event.
/// </summary>
public sealed class ControllerConnectionEvent : ControllerEvent
{
    /// <summary>
    /// The number of triggers the controller has.
    /// </summary>
    public readonly int TriggerCount;

    /// <summary>
    /// The number of thumbsticks the controller has.
    /// </summary>
    public readonly int ThumbstickCount;

    /// <summary>
    /// The number of hats the controller has.
    /// </summary>
    public readonly int HatCount;

    /// <summary>
    /// The number of axes the controller has.
    /// </summary>
    public readonly int AxisCount;

    /// <summary>
    /// Gets whether the controller has connected or not.
    /// </summary>
    public readonly bool IsConnected;

    public ControllerConnectionEvent(int index, int triggerCount, int thumbstickCount, int hatCount, int axisCount, bool isConnected)
        : base(index)
    {
        TriggerCount = triggerCount;
        ThumbstickCount = thumbstickCount;
        HatCount = hatCount;
        AxisCount = axisCount;
        IsConnected = isConnected;
    }
}
