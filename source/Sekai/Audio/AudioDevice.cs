// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

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

    public abstract void Dispose();
}
