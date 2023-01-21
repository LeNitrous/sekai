// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

public readonly struct BlendInfo : IEquatable<BlendInfo>
{
    /// <summary>
    /// The color masking flags.
    /// </summary>
    public readonly BlendingMask Masking;

    /// <summary>
    /// The blending parameters.
    /// </summary>
    public readonly BlendingParameters Parameters;

    public BlendInfo(BlendingMask masking, BlendingParameters parameters)
    {
        Masking = masking;
        Parameters = parameters;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlendInfo info && Equals(info);
    }

    public bool Equals(BlendInfo other)
    {
        return Masking == other.Masking && Parameters.Equals(other.Parameters);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Masking, Parameters);
    }

    public static bool operator ==(BlendInfo left, BlendInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlendInfo left, BlendInfo right)
    {
        return !(left == right);
    }
}
