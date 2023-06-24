// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System.Runtime.CompilerServices;

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of how many times a texture is sampled.
/// </summary>
public enum TextureSampleCount
{
    /// <summary>
    /// A texture is sampled once.
    /// </summary>
    Count1,

    /// <summary>
    /// A texture is sampled 2 times.
    /// </summary>
    Count2,

    /// <summary>
    /// A texture is sampled 4 times.
    /// </summary>
    Count4,

    /// <summary>
    /// A texture is sampled 8 times.
    /// </summary>
    Count8,

    /// <summary>
    /// A texture is sampled 16 times.
    /// </summary>
    Count16,

    /// <summary>
    /// A texture is sampled 32 times.
    /// </summary>
    Count32,
}

public static class TextureSampleCountExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetSampleCount(this TextureSampleCount count) => count switch
    {
        TextureSampleCount.Count2 => 2,
        TextureSampleCount.Count4 => 4,
        TextureSampleCount.Count8 => 8,
        TextureSampleCount.Count16 => 16,
        TextureSampleCount.Count32 => 32,
        _ => 1,
    };
}
