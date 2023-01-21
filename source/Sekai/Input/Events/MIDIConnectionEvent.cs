// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input.Events;

/// <summary>
/// Represents a MIDI connection event.
/// </summary>
public sealed class MIDIConnectionEvent : MIDIEvent
{
    /// <summary>
    /// Gets whether the MIDI device has connected or not. 
    /// </summary>
    public readonly bool IsConnected;

    public MIDIConnectionEvent(int index, bool isConnected)
        : base(index)
    {
        IsConnected = isConnected;
    }
}
