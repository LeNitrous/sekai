// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;

namespace Sekai.Audio;

public abstract class AudioSystem : FrameworkObject
{
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
