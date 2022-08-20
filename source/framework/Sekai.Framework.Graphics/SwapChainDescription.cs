// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct SwapChainDescription : IEquatable<SwapChainDescription>
{
    /// <summary>
    /// The target of all rendering operations.
    /// </summary>
    public SwapChainSource Source;

    /// <summary>
    /// The format of the depth target.
    /// </summary>
    public PixelFormat? DepthTargetFormat;


    /// <summary>
    /// The initial width of the swap chain.
    /// </summary>
    public uint Width;

    /// <summary>
    /// The initial height of the swap chain.
    /// </summary>
    public uint Height;

    /// <summary>
    /// Whether to sync the presentation to the window system's vertical refresh rate.
    /// </summary>
    public bool VerticalSync;

    /// <summary>
    /// Whether the color target of the Swapchain will use an sRGB PixelFormat.
    /// </summary>
    public bool ColorSRGB;

    public SwapChainDescription(SwapChainSource source, PixelFormat? depthTargetFormat, uint width, uint height, bool verticalSync, bool colorSrgb)
    {
        Source = source;
        DepthTargetFormat = depthTargetFormat;
        Width = width;
        Height = height;
        VerticalSync = verticalSync;
        ColorSRGB = colorSrgb;
    }

    public override bool Equals(object? obj)
    {
        return obj is SwapChainDescription other && Equals(other);
    }

    public bool Equals(SwapChainDescription other)
    {
        return Source.Equals(other.Source) &&
               Width == other.Width &&
               Height == other.Height &&
               VerticalSync == other.VerticalSync &&
               ColorSRGB == other.ColorSRGB &&
               EqualityComparer<PixelFormat?>.Default.Equals(DepthTargetFormat, other.DepthTargetFormat);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Source);
        hash.Add(Width);
        hash.Add(Height);
        hash.Add(VerticalSync);
        hash.Add(ColorSRGB);
        hash.Add(DepthTargetFormat);
        return hash.ToHashCode();
    }

    public static bool operator ==(SwapChainDescription left, SwapChainDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SwapChainDescription left, SwapChainDescription right)
    {
        return !(left == right);
    }
}
