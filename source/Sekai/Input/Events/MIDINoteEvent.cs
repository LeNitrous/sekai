// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Input.Devices.MIDI;

namespace Sekai.Input.Events;

/// <summary>
/// Represents a MIDI note event.
/// </summary>
public sealed class MIDINoteEvent : MIDIEvent
{
    /// <summary>
    /// The affected note.
    /// </summary>
    public readonly Note Note;

    /// <summary>
    /// The note's state.
    /// </summary>
    public readonly bool IsPressed;

    public MIDINoteEvent(int index, Note note, bool isPressed)
        : base(index)
    {
        Note = note;
        IsPressed = isPressed;
    }
}
