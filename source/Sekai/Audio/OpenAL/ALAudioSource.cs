// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Silk.NET.OpenAL;

namespace Sekai.Audio.OpenAL;

internal sealed class ALAudioSource : AudioSource
{
    public override bool Loop
    {
        get
        {
            AL.GetSourceProperty(handle, SourceBoolean.Looping, out bool value);
            return value;
        }
        set => AL.SetSourceProperty(handle, SourceBoolean.Looping, value);
    }

    public override float Pitch
    {
        get
        {
            AL.GetSourceProperty(handle, SourceFloat.Pitch, out float value);
            return value;
        }
        set => AL.SetSourceProperty(handle, SourceFloat.Pitch, value);
    }

    public override float Volume
    {
        get
        {
            AL.GetSourceProperty(handle, SourceFloat.Gain, out float value);
            return value;
        }
        set => AL.SetSourceProperty(handle, SourceFloat.Gain, value);
    }

    public override Vector3 Position
    {
        get
        {
            AL.GetSourceProperty(handle, SourceVector3.Position, out var value);
            return value;
        }
        set => AL.SetSourceProperty(handle, SourceVector3.Position, value);
    }

    public override Vector3 Velocity
    {
        get
        {
            AL.GetSourceProperty(handle, SourceVector3.Velocity, out var value);
            return value;
        }
        set => AL.SetSourceProperty(handle, SourceVector3.Velocity, value);
    }

    public override Vector3 Direction
    {
        get
        {
            AL.GetSourceProperty(handle, SourceVector3.Direction, out var value);
            return value;
        }
        set => AL.SetSourceProperty(handle, SourceVector3.Direction, value);
    }

    public override AudioSourceState State
    {
        get
        {
            AL.GetSourceProperty(handle, GetSourceInteger.SourceState, out int value);

            switch ((SourceState)value)
            {
                case SourceState.Initial:
                case SourceState.Stopped:
                    return AudioSourceState.Stopped;

                case SourceState.Playing:
                    return AudioSourceState.Playing;

                case SourceState.Paused:
                    return AudioSourceState.Paused;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }

#pragma warning disable IDE1006

    private readonly AL AL;

#pragma warning restore IDE1006

    private bool isDisposed;
    private readonly uint handle;
    private readonly Dictionary<uint, ALAudioBuffer> buffersOwned = new();

    public ALAudioSource(AL al)
    {
        AL = al;
        handle = AL.GenSource();
    }

    public override unsafe void Clear()
    {
        AL.GetSourceProperty(handle, GetSourceInteger.SourceType, out int type);

        if ((SourceType)type == SourceType.Static)
            throw new InvalidOperationException("Cannot play from a buffer at this current state.");

        AL.GetSourceProperty(handle, GetSourceInteger.BuffersProcessed, out int processed);

        if (processed <= 0)
        {
            return;
        }

        Span<uint> bufferIds = stackalloc uint[processed];

        fixed (uint* pointer = bufferIds)
        {
            AL.SourceUnqueueBuffers(handle, processed, pointer);
        }

        for (int i = 0; i < bufferIds.Length; i++)
        {
            buffersOwned.Remove(bufferIds[i]);
        }
    }

    public override unsafe AudioBuffer Dequeue()
    {
        AL.GetSourceProperty(handle, GetSourceInteger.SourceType, out int type);

        if ((SourceType)type == SourceType.Static)
            throw new InvalidOperationException("Cannot play from a buffer at this current state.");

        AL.GetSourceProperty(handle, GetSourceInteger.BuffersProcessed, out int processed);

        if (processed <= 0)
            throw new InvalidOperationException("There are no buffers to dequeue.");

        Span<uint> bufferIds = stackalloc uint[1];

        fixed (uint* pointer = bufferIds)
        {
            AL.SourceUnqueueBuffers(handle, 1, pointer);
        }

        if (!buffersOwned.TryGetValue(bufferIds[0], out var buffer))
            throw new InvalidOperationException("Dequeued a buffer not owned by this source.");

        return buffer;
    }

    public override unsafe void Enqueue(AudioBuffer buffer)
    {
        AL.GetSourceProperty(handle, GetSourceInteger.SourceType, out int type);

        if ((SourceType)type == SourceType.Static)
            throw new InvalidOperationException("Cannot play from a buffer at this current state.");

        var b = (ALAudioBuffer)buffer;

        Span<uint> bufferIds = stackalloc uint[1] { (uint)b };
        buffersOwned.Add(bufferIds[0], b);

        fixed (uint* pointer = bufferIds)
        {
            AL.SourceQueueBuffers(handle, 1, pointer);
        }
    }

    public override void Play(AudioBuffer buffer)
    {
        AL.GetSourceProperty(handle, GetSourceInteger.SourceType, out int type);

        if ((SourceType)type != SourceType.Undetermined)
            throw new InvalidOperationException("Cannot play from a buffer at this current state.");

        AL.SetSourceProperty(handle, SourceInteger.Buffer, (uint)(ALAudioBuffer)buffer);

        AL.SourcePlay(handle);

        while (State == AudioSourceState.Playing) ;

        AL.SetSourceProperty(handle, SourceInteger.Buffer, 0);
    }

    public override void Play()
    {
        AL.GetSourceProperty(handle, GetSourceInteger.SourceType, out int type);

        if ((SourceType)type == SourceType.Static)
            throw new InvalidOperationException("Cannot play from a buffer at this current state.");

        AL.SourcePlay(handle);
    }

    public override void Pause()
    {
        AL.SourcePause(handle);
    }

    public override void Stop()
    {
        AL.SourceStop(handle);
    }

    ~ALAudioSource()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        AL.DeleteSource(handle);

        isDisposed = true;

        GC.SuppressFinalize(this);
    }
}
