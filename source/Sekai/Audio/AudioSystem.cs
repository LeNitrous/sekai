// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Audio;

/// <summary>
/// The system responsible for interacting with low level audio-related functionality.
/// </summary>
public abstract class AudioSystem : DisposableObject
{
    /// <summary>
    /// Gets the audio system's name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the audio system's version.
    /// </summary>
    public abstract Version Version { get; }

    /// <summary>
    /// Gets or sets the audio output device.
    /// </summary>
    /// <remarks>
    /// The "Default" device is the OS-specified audio device.
    /// </remarks>
    public abstract string Device { get; set; }

    /// <summary>
    /// Enumerates all available devices on this system.
    /// </summary>
    public abstract IEnumerable<string> Devices { get; }

    /// <summary>
    /// Gets all available extensions for the audio system.
    /// </summary>
    public abstract IReadOnlyList<string> Extensions { get; }

    /// <summary>
    /// Creates an audio buffer.
    /// </summary>
    public abstract NativeAudioBuffer CreateBuffer();

    /// <summary>
    /// Creates an audio source.
    /// </summary>
    public abstract NativeAudioSource CreateSource();

    /// <summary>
    /// Creates an audio listener.
    /// </summary>
    public abstract NativeAudioListener CreateListener();

    /// <summary>
    /// The default device name for the audio system.
    /// </summary>
    public static readonly string DEFAULT_DEVICE = @"Default";
}
