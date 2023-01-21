// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;

namespace Sekai.Audio;

/// <summary>
/// An interface exposing track information and controls.
/// </summary>
public interface IAudioController : IDisposable
{
    /// <summary>
    /// Gets or sets the position of the track.
    /// </summary>
    TimeSpan Position { get; set; }

    /// <summary>
    /// Gets the duration of the track.
    /// </summary>
    TimeSpan Duration { get; }

    /// <summary>
    /// The playable duration of the track.
    /// </summary>
    TimeSpan Available { get; }

    /// <summary>
    /// The position of the track in 3D space.
    /// </summary>
    Vector3 PositionSpatial { get; set; }

    /// <summary>
    /// Gets whether the track is currently playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Gets or sets the looping state of the track.
    /// </summary>
    bool Loop { get; set; }

    /// <summary>
    /// Plays the track from the start.
    /// </summary>
    void Play();

    /// <summary>
    /// Stops the track's playback.
    /// </summary>
    void Stop();

    /// <summary>
    /// Pauses the track.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the track.
    /// </summary>
    void Resume();
}
