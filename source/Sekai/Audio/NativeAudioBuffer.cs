// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Audio;

/// <summary>
/// A write-only audio buffer.
/// </summary>
public abstract class NativeAudioBuffer : DisposableObject
{
    /// <summary>
    /// The buffer's size.
    /// </summary>
    public abstract int Size { get; }

    /// <summary>
    /// The buffer's sample rate.
    /// </summary>
    public abstract int SampleRate { get; }

    /// <summary>
    /// The buffer's format.
    /// </summary>
    public abstract AudioFormat Format { get; }

    /// <summary>
    /// Sets the data on this audio buffer rewriting existing content.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="size">The size of the data.</param>
    /// <param name="format">The data's format.</param>
    /// <param name="sampleRate">The data's sample rate.</param>
    public abstract void SetData(nint data, int size, AudioFormat format, int sampleRate);
}
