// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Audio;

namespace Sekai.Null.Audio;

internal class NullAudioSource : NativeAudioSource
{
    public override bool Loop { get; set; }
    public override float Pitch { get; set; } = 1.0f;
    public override float Volume { get; set; } = 1.0f;
    public override Vector3 Position { get; set; }
    public override AudioSourceState State { get; }

    public override void Clear()
    {
    }

    public override NativeAudioBuffer Dequeue()
    {
        return new NullAudioBuffer();
    }

    public override void Enqueue(NativeAudioBuffer buffer)
    {
    }

    public override void Pause()
    {
    }

    public override void Play()
    {
    }

    public override void Reset()
    {
    }

    public override void Stop()
    {
    }
}
