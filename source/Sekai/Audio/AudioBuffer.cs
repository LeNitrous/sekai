using System;
using System.Runtime.CompilerServices;

namespace Sekai.Audio;

/// <summary>
/// A buffer that contains audio data that can be used by <see cref="AudioSource"/>s.
/// </summary>
public class AudioBuffer : FrameworkObject
{
    private readonly IAudioSystem audio = Game.Resolve<IAudioSystem>();
    private readonly INativeAudioBuffer buffer;

    public AudioBuffer()
    {
        buffer = audio.CreateBuffer();
    }

    /// <summary>
    /// Sets the data of this audio buffer.
    /// </summary>
    /// <param name="data">The data to be set.</param>
    /// <param name="size">The size of the data to be set.</param>
    /// <param name="format">The audio format of the data.</param>
    /// <param name="sampleRate">The audio data sample rate.</param>
    public void SetData(nint data, int size, AudioFormat format, int sampleRate)
    {
        buffer.SetData(data, size, format, sampleRate);
    }

    /// <inheritdoc cref="SetData"/>
    public unsafe void SetData<T>(ReadOnlySpan<T> data, AudioFormat format, int sampleRate)
        where T : unmanaged
    {
        fixed (T* ptr = data)
            SetData((nint)ptr, Unsafe.SizeOf<T>() * data.Length, format, sampleRate);
    }

    /// <inheritdoc cref="SetData"/>
    public void Update<T>(T[] data, AudioFormat format, int sampleRate)
        where T : unmanaged
    {
        SetData<T>(data.AsSpan(), format, sampleRate);
    }
}
