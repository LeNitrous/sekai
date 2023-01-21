// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Sekai.Audio;
using Silk.NET.OpenAL;

namespace Sekai.OpenAL;

internal sealed class ALAudioSource : NativeAudioSource
{
    public override bool Loop
    {
        get
        {
            system.GetSourceProperty(this, SourceBoolean.Looping, out bool value);
            return value;
        }
        set => system.SetSourceProperty(this, SourceBoolean.Looping, value);
    }

    public override float Pitch
    {
        get
        {
            system.GetSourceProperty(this, SourceFloat.Pitch, out float value);
            return value;
        }
        set => system.SetSourceProperty(this, SourceFloat.Pitch, MathF.Abs(value));
    }

    public override float Volume
    {
        get
        {
            system.GetSourceProperty(this, SourceFloat.Gain, out float value);
            return value;
        }
        set => system.SetSourceProperty(this, SourceFloat.Gain, MathF.Abs(value));
    }

    public override AudioSourceState State
    {
        get
        {
            system.GetSourceProperty(this, GetSourceInteger.SourceState, out int value);

            switch ((SourceState)value)
            {
                case SourceState.Initial:
                case SourceState.Stopped:
                default:
                    return AudioSourceState.Stopped;

                case SourceState.Playing:
                    return AudioSourceState.Playing;

                case SourceState.Paused:
                    return AudioSourceState.Paused;
            }
        }
    }

    public override unsafe Vector3 Position
    {
        get
        {
            system.GetSourceProperty(this, SourceVector3.Position, out var value);
            return value;
        }
        set => system.SetSourceProperty(this, SourceVector3.Position, (float*)Unsafe.AsPointer(ref value));
    }

    private readonly uint sourceId;
    private readonly ALAudioSystem system;

    public ALAudioSource(ALAudioSystem system, uint sourceId)
    {
        this.system = system;
        this.sourceId = sourceId;
    }

    public override void Play() => system.SourcePlay(this);

    public override void Stop() => system.SourceStop(this);

    public override void Pause() => system.SourcePause(this);

    public override void Reset() => system.SourceRewind(this);

    public override void Clear() => system.SourceClear(this);

    public override void Enqueue(NativeAudioBuffer buffer) => system.SourceEnqueue(this, buffer);

    public override NativeAudioBuffer Dequeue() => system.SourceDequeue(this);

    protected override void Destroy() => system.DestroySource(this);

    public static implicit operator uint(ALAudioSource source) => source.sourceId;
}
