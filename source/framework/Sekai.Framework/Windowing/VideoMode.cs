// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;

namespace Sekai.Framework.Windowing;

public struct VideoMode : IEquatable<VideoMode>
{
    public int RefreshRate { get; }
    public Size Resolution { get; }
    public int BitsPerPixel { get; }

    public VideoMode(Size resolution, int refreshRate, int bitsPerPixel)
    {
        Resolution = resolution;
        RefreshRate = refreshRate;
        BitsPerPixel = bitsPerPixel;
    }

    public override string ToString()
    {
        return $"{{Resolution: {Resolution.Width} x {Resolution.Height}, Refresh Rate: {RefreshRate}, Bits Per Pixel: {BitsPerPixel}}}";
    }

    public bool Equals(VideoMode other)
    {
        return RefreshRate.Equals(other.RefreshRate) &&
            Resolution.Equals(other.Resolution) &&
            BitsPerPixel.Equals(other.BitsPerPixel);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RefreshRate, Resolution, BitsPerPixel);
    }

    public override bool Equals(object? obj)
    {
        return obj is VideoMode mode && Equals(mode);
    }

    public static bool operator ==(VideoMode left, VideoMode right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VideoMode left, VideoMode right)
    {
        return !(left == right);
    }
}
