using System;
using Sekai.Audio;
using Silk.NET.OpenAL;

namespace Sekai.OpenAL;

internal class ALAudioBuffer : FrameworkObject, INativeAudioBuffer
{
    private readonly uint bufferId;
    internal readonly ALAudioSystem audio;

    public ALAudioBuffer(ALAudioSystem audio)
    {
        bufferId = audio.AL.GenBuffer();
        this.audio = audio;
    }

    public unsafe void SetData(nint data, int size, AudioFormat format, int sampleRate)
    {
        var fmt = format switch
        {
            AudioFormat.Mono8 => BufferFormat.Mono8,
            AudioFormat.Mono16 => BufferFormat.Mono16,
            AudioFormat.Stereo8 => BufferFormat.Stereo8,
            AudioFormat.Stereo16 => BufferFormat.Stereo16,
            _ => throw new NotSupportedException(@$"Audio format ""{format}"" is not supported.")
        };

        audio.AL.BufferData(bufferId, fmt, (void*)data, size, sampleRate);
    }
}
