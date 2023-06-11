// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Audio;

/// <summary>
/// Determines the audio format.
/// </summary>
public enum AudioFormat
{
    /// <summary>
    /// 8-bit single channel audio.
    /// </summary>
    Mono8,

    /// <summary>
    /// 16-bit single channel audio.
    /// </summary>
    Mono16,

    /// <summary>
    /// 8-bit dual channel audio.
    /// </summary>
    Stereo8,

    /// <summary>
    /// 16-bit dual channel audio.
    /// </summary>
    Stereo16,
}

public static class AudioFormatExtensions
{
    /// <summary>
    /// Gets whether the audio format is stereo.
    /// </summary>
    public static bool IsStereo(this AudioFormat format)
    {
        switch (format)
        {
            case AudioFormat.Mono8:
            case AudioFormat.Mono16:
                return false;

            case AudioFormat.Stereo8:
            case AudioFormat.Stereo16:
                return true;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    /// <summary>
    /// Gets the number of channels of the given format.
    /// </summary>
    public static int GetChannelCount(this AudioFormat format)
    {
        return IsStereo(format) ? 2 : 1;
    }

    /// <summary>
    /// Gets the bits per sample of the given format.
    /// </summary>
    public static int GetBitsPerSample(this AudioFormat format)
    {
        switch (format)
        {
            case AudioFormat.Mono8:
            case AudioFormat.Stereo8:
                return 8;

            case AudioFormat.Mono16:
            case AudioFormat.Stereo16:
                return 16;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}
