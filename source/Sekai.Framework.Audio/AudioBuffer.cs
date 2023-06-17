// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;

namespace Sekai.Framework.Audio;

/// <summary>
/// A buffer that stores audio-related data.
/// </summary>
public abstract class AudioBuffer : IDisposable
{
    /// <summary>
    /// Gets the capacity of this buffer.
    /// </summary>
    public abstract int Capacity { get; }

    /// <summary>
    /// Gets the sample rate of this buffer.
    /// </summary>
    public abstract int SampleRate { get; }

    /// <summary>
    /// Gets the audio format of this buffer.
    /// </summary>
    public abstract AudioFormat Format { get; }

    /// <summary>
    /// Gets the duration of this buffer.
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromSeconds(Capacity / (SampleRate * Format.GetChannelCount() * (Format.GetBitsPerSample() / 8)));

    /// <summary>
    /// Sets the data on this buffer.
    /// </summary>
    /// <param name="data">The data to set.</param>
    /// <param name="size">The size of the data to set.</param>
    /// <param name="format">The format of the data.</param>
    /// <param name="sampleRate">The sample rate of the data.</param>
    public abstract void SetData(nint data, uint size, AudioFormat format, int sampleRate);

    /// <inheritdoc cref="SetData(nint, uint, AudioFormat, int)"/>
    /// <typeparam name="T">The data type to upload to the buffer.</typeparam>
    public void SetData<T>(T[] data, AudioFormat format, int sampleRate)
        where T : unmanaged
    {
        SetData((ReadOnlySpan<T>)data.AsSpan(), format, sampleRate);
    }

    /// <inheritdoc cref="SetData{T}(T[], AudioFormat, int)"/>
    public unsafe void SetData<T>(ReadOnlySpan<T> data, AudioFormat format, int sampleRate)
        where T : unmanaged
    {
        fixed (void* handle = data)
            SetData((nint)handle, (uint)(Unsafe.SizeOf<T>() * data.Length), format, sampleRate);
    }

    public abstract void Dispose();
}
