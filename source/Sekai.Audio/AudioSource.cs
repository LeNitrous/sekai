// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Sekai.Audio;

/// <summary>
/// An audio source.
/// </summary>
public abstract class AudioSource : IDisposable
{
    /// <summary>
    /// Gets or sets whether this source should loop playback.
    /// </summary>
    public abstract bool Loop { get; set; }

    /// <summary>
    /// Gets or sets the pitch of the source.
    /// </summary>
    public abstract float Pitch { get; set; }

    /// <summary>
    /// Gets or sets the volume of the source.
    /// </summary>
    public abstract float Volume { get; set; }

    /// <summary>
    /// Gets or sets the position of the source.
    /// </summary>
    public abstract Vector3 Position { get; set; }

    /// <summary>
    /// Gets or sets the velocity of the source.
    /// </summary>
    public abstract Vector3 Velocity { get; set; }

    /// <summary>
    /// Gets or sets the direction of the source.
    /// </summary>
    public abstract Vector3 Direction { get; set; }

    /// <summary>
    /// Gets the state of the source.
    /// </summary>
    public abstract AudioSourceState State { get; }

    /// <summary>
    /// Plays the source from queued <see cref="AudioBuffer"/>s.
    /// </summary>
    public abstract void Play();

    /// <summary>
    /// Plays the source using a provided <see cref="AudioBuffer"/>.
    /// </summary>
    /// <param name="buffer">The buffer to play.</param>
    /// <remarks>
    /// This call is blocking. See <see cref="Play"/> for a non-blocking operation.
    /// </remarks>
    public abstract void Play(AudioBuffer buffer);

    /// <summary>
    /// Stops the source.
    /// </summary>
    public abstract void Stop();

    /// <summary>
    /// Pauses the source.
    /// </summary>
    public abstract void Pause();

    /// <summary>
    /// Clears the source's buffer queue.
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Enqueues an audio buffer to this source.
    /// </summary>
    /// <param name="buffer">The buffer to enqueue.</param>
    public abstract void Enqueue(AudioBuffer buffer);

    /// <summary>
    /// Dequeues an audio buffer from this source.
    /// </summary>
    /// <returns>The dequeued buffer.</returns>
    public abstract AudioBuffer Dequeue();

    /// <summary>
    /// Attempts to dequeue an audio buffer from this source.
    /// </summary>
    /// <param name="buffer">The dequeued buffer.</param>
    /// <returns><see langword="true"/> if a buffer was dequeued. Otherwise <see langword="false"/></returns>
    public bool TryDequeue([NotNullWhen(true)] out AudioBuffer? buffer)
    {
        try
        {
            buffer = Dequeue();
            return true;
        }
        catch
        {
            buffer = null;
            return false;
        }
    }

    public abstract void Dispose();
}
