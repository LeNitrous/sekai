// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public readonly struct DepthInfo : IEquatable<DepthInfo>
{
    /// <summary>
    /// Whether depth testing should occur.
    /// </summary>
    public readonly bool DepthTest;

    /// <summary>
    /// Whether to write to the depth buffer if the depth test passed.
    /// </summary>
    public readonly bool WriteDepth;

    /// <summary>
    /// The depth test function.
    /// </summary>
    public readonly ComparisonKind Function;

    public DepthInfo(bool depthTest = true, bool writeDepth = true, ComparisonKind function = ComparisonKind.LessThan)
    {
        Function = function;
        DepthTest = depthTest;
        WriteDepth = writeDepth;
    }

    public override bool Equals(object? obj)
    {
        return obj is DepthInfo info && Equals(info);

    }

    public bool Equals(DepthInfo other)
    {
        return DepthTest == other.DepthTest &&
               WriteDepth == other.WriteDepth &&
               Function == other.Function;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DepthTest, WriteDepth, Function);
    }

    public static bool operator ==(DepthInfo left, DepthInfo right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DepthInfo left, DepthInfo right)
    {
        return !(left == right);
    }
}
