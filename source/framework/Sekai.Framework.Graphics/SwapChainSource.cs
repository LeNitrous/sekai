// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using Sekai.Framework.Windowing;

namespace Sekai.Framework.Graphics;

public struct SwapChainSource : IEquatable<SwapChainSource>
{
    public INativeWindow Source;

    public SwapChainSource(INativeWindow source)
    {
        Source = source;
    }

    public override bool Equals(object? obj)
    {
        return obj is SwapChainSource source && Equals(source);
    }

    public bool Equals(SwapChainSource other)
    {
        return EqualityComparer<INativeWindow>.Default.Equals(Source, other.Source);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Source);
    }

    public static bool operator ==(SwapChainSource left, SwapChainSource right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SwapChainSource left, SwapChainSource right)
    {
        return !(left == right);
    }
}
