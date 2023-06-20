// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Numerics;
using Sekai.Framework.Audio;

namespace Sekai.Headless.Audio;

internal sealed class DummyAudioSource : AudioSource
{
    public override bool Loop { get; set; }
    public override float Pitch { get; set; }
    public override float Volume { get; set; }
    public override Vector3 Position { get; set; }
    public override Vector3 Velocity { get; set; }
    public override Vector3 Direction { get; set; }
    public override AudioSourceState State => state;

    private AudioSourceState state;
    private readonly Queue<AudioBuffer> queue = new();

    public override void Clear()
    {
    }

    public override AudioBuffer Dequeue()
    {
        return queue.Dequeue();
    }

    public override void Dispose()
    {
    }

    public override void Enqueue(AudioBuffer buffer)
    {
        queue.Enqueue(buffer);
    }

    public override void Pause()
    {
        state = AudioSourceState.Paused;
    }

    public override void Play()
    {
        state = AudioSourceState.Playing;
    }

    public override void Play(AudioBuffer buffer)
    {
        Play();
    }

    public override void Stop()
    {
        state = AudioSourceState.Stopped;
    }
}
