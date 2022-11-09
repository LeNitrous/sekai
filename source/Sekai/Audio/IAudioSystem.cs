// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Audio;

public interface IAudioSystem : IDisposable
{
    /// <summary>
    /// The master gain.
    /// </summary>
    float Gain { get; set; }

    /// <summary>
    /// The position of the listener in three-dimensional space.
    /// </summary>
    Vector3 Position { get; set; }

    /// <summary>
    /// The velocity of the listener in three-dimensional space.
    /// </summary>
    Vector3 Velocity { get; set; }

    /// <summary>
    /// The orientation of the listener in three-dimensional space.
    /// </summary>
    Vector3 Orientation { get; set; }

    /// <summary>
    /// Creates an audio buffer.
    /// </summary>
    INativeAudioBuffer CreateBuffer();
}
