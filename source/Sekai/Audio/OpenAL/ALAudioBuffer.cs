// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Silk.NET.OpenAL;

namespace Sekai.Audio.OpenAL;

internal sealed class ALAudioBuffer : AudioBuffer
{
    public override int Capacity
    {
        get
        {
            AL.GetBufferProperty(handle, GetBufferInteger.Size, out int size);
            return size;
        }
    }

    public override int SampleRate
    {
        get
        {
            AL.GetBufferProperty(handle, GetBufferInteger.Frequency, out int rate);
            return rate;
        }
    }

    public override AudioFormat Format
    {
        get
        {
            AL.GetBufferProperty(handle, GetBufferInteger.Channels, out int channels);
            AL.GetBufferProperty(handle, GetBufferInteger.Bits, out int bits);

            return channels == 2
                ? bits == 8 ? AudioFormat.Stereo8 : AudioFormat.Stereo16
                : bits == 8 ? AudioFormat.Mono8 : AudioFormat.Mono16;
        }
    }

#pragma warning disable IDE1006

    private readonly AL AL;

#pragma warning restore IDE1006

    private bool isDisposed;
    private readonly uint handle;

    public ALAudioBuffer(AL al)
    {
        AL = al;
        handle = AL.GenBuffer();
    }

    public override unsafe void SetData(nint data, uint size, AudioFormat format, int sampleRate)
    {
        var fmt = format switch
        {
            AudioFormat.Mono8 => BufferFormat.Mono8,
            AudioFormat.Mono16 => BufferFormat.Mono16,
            AudioFormat.Stereo8 => BufferFormat.Stereo8,
            AudioFormat.Stereo16 => BufferFormat.Stereo16,
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
        };

        AL.BufferData(handle, fmt, (void*)data, (int)size, sampleRate);
    }

    ~ALAudioBuffer()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        AL.DeleteBuffer(handle);

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    public static explicit operator uint(ALAudioBuffer buffer) => buffer.handle;
}
