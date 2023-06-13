// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio.Dummy;
using Sekai.Audio.OpenAL;
using System;

namespace Sekai.Audio;

/// <summary>
/// Provides access to all audio-related functionality.
/// </summary>
public abstract class AudioDevice : IDisposable
{
    /// <summary>
    /// Gets the listener for this device.
    /// </summary>
    public abstract AudioListener Listener { get; }

    /// <summary>
    /// Creates an audio buffer.
    /// </summary>
    /// <returns>A new audio buffer.</returns>
    public abstract AudioBuffer CreateBuffer();

    /// <summary>
    /// Creates an audio source.
    /// </summary>
    /// <returns>A new audio source.</returns>
    public abstract AudioSource CreateSource();

    /// <summary>
    /// Creates an OpenAL audio device.
    /// </summary>
    /// <returns>An OpenAL audio device.</returns>
    public static AudioDevice CreateAL() => new ALAudioDevice();

    /// <summary>
    /// Creates a dummy audio device.
    /// </summary>
    /// <returns>A dummy audio device.</returns>
    public static AudioDevice CreateDummy() => new DummyAudioDevice();

    public abstract void Dispose();
}
