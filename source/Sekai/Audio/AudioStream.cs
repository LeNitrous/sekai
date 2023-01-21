// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using Sekai.Assets;

namespace Sekai.Audio;

/// <summary>
/// A stream containing pulse code modulation (PCM) audio data.
/// </summary>
public class AudioStream : Stream, IAsset
{
    /// <summary>
    /// The audio data format.
    /// </summary>
    public AudioFormat Format { get; }

    /// <summary>
    /// The audio sample rate.
    /// </summary>
    public int SampleRate { get; }

    public override bool CanRead => stream.CanRead;

    public override bool CanSeek => stream.CanSeek;

    public override bool CanWrite => stream.CanWrite;

    public override long Length => stream.Length;

    public override long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }

    private readonly MemoryStream stream;

    internal AudioStream(ReadOnlySpan<byte> data, AudioFormat format, int sampleRate)
    {
        stream = new MemoryStream(data.ToArray(), false);
        Format = format;
        SampleRate = sampleRate;
    }

    internal AudioStream(AudioStream other)
        : this (other.stream.ToArray(), other.Format, other.SampleRate)
    {
    }

    public override void Flush()
    {
        stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return stream.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        stream.Write(buffer, offset, count);
    }

    protected override void Dispose(bool disposing)
    {
        stream.Dispose();
        base.Dispose(disposing);
    }
}
