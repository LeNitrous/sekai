// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Describes a <see cref="DepthStencilState"/>.
/// </summary>
public struct DepthStencilStateDescription : IEquatable<DepthStencilStateDescription>
{
    /// <summary>
    /// Whether depth testing is enabled or not.
    /// </summary>
    public bool DepthTest;

    /// <summary>
    /// Whether to enable or disable writing to the depth buffer.
    /// </summary>
    public bool DepthMask;

    /// <summary>
    /// The dpeth comparison function to use.
    /// </summary>
    public ComparisonKind DepthComparison;

    /// <summary>
    /// Whether stencil testing is enabled or not.
    /// </summary>
    public bool StencilTest;

    /// <summary>
    /// The stencil read mask.
    /// </summary>
    public byte StencilReadMask;

    /// <summary>
    /// The stencil write mask.
    /// </summary>
    public byte StencilWriteMask;

    /// <summary>
    /// The stencil operation to perform for the front-facing pixel.
    /// </summary>
    public StencilBehaviorDescription Front;

    /// <summary>
    /// The stencil operation to perform for the back-facing pixel.
    /// </summary>
    public StencilBehaviorDescription Back;

    public DepthStencilStateDescription(bool depthTest, bool depthMask, ComparisonKind depthCompaison, bool stencilTest, byte stencilReadMask, byte stencilWriteMask, StencilBehaviorDescription stencilFront, StencilBehaviorDescription stencilBack)
    {
        DepthTest = depthTest;
        DepthMask = depthMask;
        DepthComparison = depthCompaison;
        StencilTest = stencilTest;
        StencilReadMask = stencilReadMask;
        StencilWriteMask = stencilWriteMask;
        Front = stencilFront;
        Back = stencilBack;
    }

    public DepthStencilStateDescription(bool depthTest, bool depthMask, ComparisonKind depthComparison)
    {
        DepthTest = depthTest;
        DepthMask = depthMask;
        DepthComparison = depthComparison;
        StencilTest = false;
        StencilReadMask = 0xFF;
        StencilWriteMask = 0xFF;
        Back = new(StencilOperation.Keep, StencilOperation.Increment, StencilOperation.Keep, ComparisonKind.Always);
        Front = new(StencilOperation.Keep, StencilOperation.Increment, StencilOperation.Keep, ComparisonKind.Always);
    }

    public DepthStencilStateDescription(bool stencilTest, byte stencilReadMask, byte stencilWriteMask, StencilBehaviorDescription stencilFront, StencilBehaviorDescription stencilBack)
    {
        DepthTest = false;
        DepthMask = true;
        DepthComparison = ComparisonKind.LessEqual;
        StencilTest = stencilTest;
        StencilReadMask = stencilReadMask;
        StencilWriteMask = stencilWriteMask;
        Back = stencilBack;
        Front = stencilFront;
    }

    public bool Equals(DepthStencilStateDescription other)
    {
        return DepthTest == other.DepthTest &&
               DepthMask == other.DepthMask &&
               DepthComparison == other.DepthComparison &&
               StencilTest == other.StencilTest &&
               StencilReadMask == other.StencilReadMask &&
               StencilWriteMask == other.StencilWriteMask &&
               Front.Equals(other.Front) &&
               Back.Equals(other.Back);
    }

    public override bool Equals(object? obj)
    {
        return obj is DepthStencilStateDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DepthTest, DepthMask, DepthComparison, StencilTest, StencilReadMask, StencilWriteMask, Front, Back);
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
