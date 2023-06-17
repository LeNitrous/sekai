// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// Describes actions to perform for a given stencil face.
/// </summary>
public struct StencilBehaviorDescription : IEquatable<StencilBehaviorDescription>
{
    /// <summary>
    /// The operation to perform when the stencil test fails.
    /// </summary>
    public StencilOperation StencilFail;

    /// <summary>
    /// The operation to perform when the depth test fails.
    /// </summary>
    public StencilOperation DepthFail;

    /// <summary>
    /// The operation to perform when both the depth and stencil tests pass.
    /// </summary>
    public StencilOperation StencilPass;

    /// <summary>
    /// The function to use when comparing new stencil data to the existing stencil data.
    /// </summary>
    public ComparisonKind Comparison;

    public StencilBehaviorDescription(StencilOperation stencilFail, StencilOperation depthFail, StencilOperation depthStencilPass, ComparisonKind comparison)
    {
        StencilFail = stencilFail;
        DepthFail = depthFail;
        StencilPass = depthStencilPass;
        Comparison = comparison;
    }

    public bool Equals(StencilBehaviorDescription other)
    {
        return StencilFail == other.StencilFail &&
               DepthFail == other.DepthFail &&
               StencilPass == other.StencilPass &&
               Comparison == other.Comparison;
    }

    public override bool Equals(object? obj)
    {
        return obj is StencilBehaviorDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StencilFail, DepthFail, StencilPass, Comparison);
    }

    public static bool operator ==(StencilBehaviorDescription left, StencilBehaviorDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(StencilBehaviorDescription left, StencilBehaviorDescription right)
    {
        return !(left == right);
    }
}
