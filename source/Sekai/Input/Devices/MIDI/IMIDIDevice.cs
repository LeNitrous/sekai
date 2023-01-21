// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Input.Devices.MIDI;

/// <summary>
/// Represents a MIDI device.
/// </summary>
public interface IMIDIDevice : IInputDevice
{
    /// <summary>
    /// Called when a <see cref="Note"/> has been pressed down.
    /// </summary>
    event Action<IMIDIDevice, Note>? OnNotePressed;

    /// <summary>
    /// Called when a <see cref="Note"/> has been released.
    /// </summary>
    event Action<IMIDIDevice, Note>? OnNoteRelease;
}
