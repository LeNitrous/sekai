// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;

namespace Sekai.Audio;

public sealed class AudioContext : FrameworkObject
{
    public float Gain
    {
        get => audio.Gain;
        set => audio.Gain = value;
    }

    public Vector3 Position
    {
        get => audio.Position;
        set => audio.Position = value;
    }

    public Vector3 Velocity
    {
        get => audio.Velocity;
        set => audio.Velocity = value;
    }

    public Vector3 Orientation
    {
        get => audio.Orientation;
        set => audio.Orientation = value;
    }

    private readonly IAudioSystem audio;

    public AudioContext(IAudioSystem audio)
    {
        this.audio = audio;
    }
}
