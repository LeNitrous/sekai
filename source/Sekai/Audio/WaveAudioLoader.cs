// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Text;
using Sekai.Assets;

namespace Sekai.Audio;

internal sealed class WaveAudioLoader : DisposableObject, IAssetLoader<AudioStream>
{
    public string[] Extensions { get; } = { @".wav" };

    public AudioStream Load(ReadOnlySpan<byte> bytes)
    {
        if (!MemoryExtensions.SequenceEqual(bytes[0..4], header_riff))
            throw new ArgumentException(@"Failed to find ""RIFF"" header at position 0.", nameof(bytes));

        if (!MemoryExtensions.SequenceEqual(bytes[8..12], header_wave))
            throw new ArgumentException(@"Failed to find ""WAVE"" header at position 8.", nameof(bytes));

        if (!MemoryExtensions.SequenceEqual(bytes[12..16], header_fmt))
            throw new ArgumentException(@"Failed to find ""fmt "" header at position 12.", nameof(bytes));

        if (!MemoryExtensions.SequenceEqual(bytes[36..40], header_data))
            throw new ArgumentException(@"Failed to find ""data"" header at position 36.", nameof(bytes));

        short numChannels = BitConverter.ToInt16(bytes[22..24]);
        short bitsPerSamp = BitConverter.ToInt16(bytes[34..36]);

        var format = numChannels == 2
            ? bitsPerSamp == 8 ? AudioFormat.Stereo8 : AudioFormat.Stereo16
            : bitsPerSamp == 8 ? AudioFormat.Mono8 : AudioFormat.Mono16;

        int rate = BitConverter.ToInt32(bytes[24..28]);
        int size = BitConverter.ToInt32(bytes[40..44]);

        return new AudioStream(bytes.Slice(44, size), format, rate);
    }

    private static readonly byte[] header_riff = Encoding.ASCII.GetBytes("RIFF");
    private static readonly byte[] header_wave = Encoding.ASCII.GetBytes("WAVE");
    private static readonly byte[] header_data = Encoding.ASCII.GetBytes("data");
    private static readonly byte[] header_fmt = Encoding.ASCII.GetBytes("fmt ");
}
