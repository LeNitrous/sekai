// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct DepthStencilStateDescription : IEquatable<DepthStencilStateDescription>
{
    /// <summary>
    /// Whether depth testing is enabled.
    /// </summary>
    public bool DepthTest;

    /// <summary>
    /// Whether new depth values are written to the depth buffer.
    /// </summary>
    public bool DepthWrite;

    /// <summary>
    /// The comparison function used when considering new depth values.
    /// </summary>
    public ComparisonKind DepthComparison;

    /// <summary>
    /// Whether stencil testing is enabled.
    /// </summary>
    public bool StencilTest;

    /// <summary>
    /// Controls how stencil tests are handled for pixels whose surface faces towards the camera.
    /// </summary>
    public StencilBehaviorDescription StencilFront;

    /// <summary>
    /// Controls how stencil tests are handled for pixels whose surface faces away from the camera.
    /// </summary>
    public StencilBehaviorDescription StencilBack;

    /// <summary>
    /// Controls the portion of the stencil buffer used for reading.
    /// </summary>
    public byte StencilReadMask;

    /// <summary>
    /// Controls the portion of the stencil buffer used for writing.
    /// </summary>
    public byte StencilWriteMask;

    /// <summary>
    /// The reference value to use when doing a stencil test.
    /// </summary>
    public uint StencilReference;

    public DepthStencilStateDescription(bool depthTest, bool depthWrite, ComparisonKind depthComparison, bool stencilTest, StencilBehaviorDescription stencilFront, StencilBehaviorDescription stencilBack, byte stencilReadMask, byte stencilWriteMask, uint stencilReference)
    {
        DepthTest = depthTest;
        DepthWrite = depthWrite;
        DepthComparison = depthComparison;
        StencilTest = stencilTest;
        StencilFront = stencilFront;
        StencilBack = stencilBack;
        StencilReadMask = stencilReadMask;
        StencilWriteMask = stencilWriteMask;
        StencilReference = stencilReference;
    }

    public override bool Equals(object? obj)
    {
        return obj is DepthStencilStateDescription other && Equals(other);
    }

    public bool Equals(DepthStencilStateDescription other)
    {
        return DepthTest == other.DepthTest &&
               DepthWrite == other.DepthWrite &&
               DepthComparison == other.DepthComparison &&
               StencilTest == other.StencilTest &&
               StencilFront.Equals(other.StencilFront) &&
               StencilBack.Equals(other.StencilBack) &&
               StencilReadMask == other.StencilReadMask &&
               StencilWriteMask == other.StencilWriteMask &&
               StencilReference == other.StencilReference;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(DepthTest);
        hash.Add(DepthWrite);
        hash.Add(DepthComparison);
        hash.Add(StencilTest);
        hash.Add(StencilFront);
        hash.Add(StencilBack);
        hash.Add(StencilReadMask);
        hash.Add(StencilWriteMask);
        hash.Add(StencilReference);
        return hash.ToHashCode();
    }

    public static bool operator ==(DepthStencilStateDescription left, DepthStencilStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(DepthStencilStateDescription left, DepthStencilStateDescription right)
    {
        return !(left == right);
    }
}
