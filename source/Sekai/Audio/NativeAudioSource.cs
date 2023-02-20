// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Sekai.Audio;

public abstract class NativeAudioSource : DisposableObject
{
    /// <summary>
    /// Gets or sets whether this source should loop.
    /// </summary>
    public abstract bool Loop { get; set; }

    /// <summary>
    /// Gets or sets the pitch of this source.
    /// </summary>
    public abstract float Pitch { get; set; }

    /// <summary>
    /// Gets or sets the volume of this source.
    /// </summary>
    public abstract float Volume { get; set; }

    /// <summary>
    /// Gets or sets the position of this source in 3D space.
    /// </summary>
    public abstract Vector3 Position { get; set; }

    /// <summary>
    /// Gets the state of the audio source.
    /// </summary>
    public abstract AudioSourceState State { get; }

    /// <summary>
    /// Plays the audio source.
    /// </summary>
    public abstract void Play();

    /// <summary>
    /// Stops the audio source.
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Pauses the audio source.
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// Resets the audio source.
    /// </summary>
    public abstract void Reset();

    /// <summary>
    /// Enqueues a buffer for processing.
    /// </summary>
    /// <param name="buffers">The buffers to be enqueued.</param>
    public abstract void Enqueue(NativeAudioBuffer buffer);

    /// <summary>
    /// Dequeues a buffer from processing.
    /// </summary>
    /// <returns>The dequeued buffer.</returns>
    public abstract NativeAudioBuffer Dequeue();

    /// <summary>
    /// Clears all buffers from processing.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Tries to dequeue a buffer from processing.
    /// </summary>
    /// <param name="buffer">The dequeued buffer.</param>
    /// <returns>True if a buffer as been dequeued. False otherwise.</returns>
    public bool TryDequeue([NotNullWhen(true)] out NativeAudioBuffer? buffer)
    {
        try
        {
            buffer = Dequeue();
            return true;
        }
        catch (InvalidOperationException)
        {
            buffer = null;
            return false;
        }
    }
}
