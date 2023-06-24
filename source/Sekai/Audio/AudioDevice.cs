// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Audio;

/// <summary>
/// Provides access to all audio-related functionality.
/// </summary>
public abstract class AudioDevice : IDisposable
{
    /// <summary>
    /// The default audio device.
    /// </summary>
    public const string DEFAULT_AUDIO_DEVICE = "Default";

    /// <summary>
    /// The backend audio API used.
    /// </summary>
    public abstract AudioAPI API { get; }

    /// <summary>
    /// The device's name.
    /// </summary>
    public abstract string Device { get; set; }

    /// <summary>
    /// The device's version.
    /// </summary>
    public abstract Version Version { get; }

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
    /// Starts processing of this audio device.
    /// </summary>
    public abstract void Process();

    /// <summary>
    /// Suspends processing of this audio device.
    /// </summary>
    public abstract void Suspend();

    /// <summary>
    /// Gets an enumeration of available devices.
    /// </summary>
    /// <returns>An enumeration of available devices.</returns>
    public abstract IEnumerable<string> GetAvailableDevices();

    public abstract void Dispose();
}
