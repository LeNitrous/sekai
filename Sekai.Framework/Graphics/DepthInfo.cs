// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Information determining how depth should be handled.
/// </summary>
public struct DepthInfo : IEquatable<DepthInfo>
{
    /// <summary>
    /// The default depth info.
    /// </summary>
    public static DepthInfo Default => new(true, false);

    /// <summary>
    /// Whether depth testing should occur.
    /// </summary>
    public readonly bool DepthTest;

    /// <summary>
    /// Whether to write to the depth buffer if tests pass.
    /// </summary>
    public readonly bool WriteDepth;

    /// <summary>
    /// The depth test comparison function.
    /// </summary>
    public readonly ComparisonKind Comparison;

    public DepthInfo(bool depthTest, bool writeDepth, ComparisonKind comparison = ComparisonKind.Always)
    {
        DepthTest = depthTest;
        WriteDepth = writeDepth;
        Comparison = comparison;
    }

    public bool Equals(DepthInfo other)
    {
        return DepthTest == other.DepthTest
            && WriteDepth == other.WriteDepth
            && Comparison == other.Comparison;
    }

    public override bool Equals(object? obj) => obj is DepthInfo info && Equals(info);

    public override int GetHashCode() => HashCode.Combine(DepthTest, WriteDepth, Comparison);

    public static bool operator ==(DepthInfo left, DepthInfo right) => left.Equals(right);

    public static bool operator !=(DepthInfo left, DepthInfo right) => !(left == right);
}
