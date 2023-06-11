// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Audio;

/// <summary>
/// The possible states of an <see cref="AudioSource"/>.
/// </summary>
public enum AudioSourceState
{
    /// <summary>
    /// The source is stopped.
    /// </summary>
    Stopped,

    /// <summary>
    /// The source is playing.
    /// </summary>
    Playing,

    /// <summary>
    /// The source is paused.
    /// </summary>
    Paused,
}
