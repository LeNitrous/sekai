// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Audio;
using Silk.NET.OpenAL;

namespace Sekai.OpenAL;

internal sealed class ALAudioBuffer : NativeAudioBuffer
{
    public override int Size
    {
        get
        {
            system.GetBufferProperty(this, GetBufferInteger.Size, out int size);
            return size;
        }
    }

    public override int SampleRate
    {
        get
        {
            system.GetBufferProperty(this, GetBufferInteger.Frequency, out int rate);
            return rate;
        }
    }

    public override AudioFormat Format
    {
        get
        {
            system.GetBufferProperty(this, GetBufferInteger.Channels, out int channels);
            system.GetBufferProperty(this, GetBufferInteger.Bits, out int bits);

            return channels == 2
                ? bits == 8 ? AudioFormat.Stereo8 : AudioFormat.Stereo16
                : bits == 8 ? AudioFormat.Mono8 : AudioFormat.Mono16;
        }
    }

    private readonly ALAudioSystem system;
    private readonly uint bufferId;

    public ALAudioBuffer(ALAudioSystem system, uint bufferId)
    {
        this.system = system;
        this.bufferId = bufferId;
    }

    public override void SetData(nint data, int size, AudioFormat format, int sampleRate)
    {
        system.SetBufferData(this, data, size, format, sampleRate);
    }

    protected override void Dispose(bool disposing)
    {
        system.DestroyBuffer(this);
    }

    public static implicit operator uint(ALAudioBuffer buffer) => buffer.bufferId;
}
