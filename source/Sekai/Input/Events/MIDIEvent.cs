// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

/// <summary>
/// Represents a MIDI device event.
/// </summary>
public abstract class MIDIEvent : DeviceEvent
{
    /// <summary>
    /// The index of the MIDI device.
    /// </summary>
    public readonly int Index;

    protected MIDIEvent(int index)
    {
        Index = index;
    }
}
