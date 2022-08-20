// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct StencilBehaviorDescription : IEquatable<StencilBehaviorDescription>
{
    /// <summary>
    /// The operation performed on samples taht fail the stencil test.
    /// </summary>
    public StencilOperation Fail;

    /// <summary>
    /// The operation performed on samples taht pass the stencil test.
    /// </summary>
    public StencilOperation Pass;

    /// <summary>
    /// The operation performed on sampels that pass the stencil test but fail the depth test.
    /// </summary>
    public StencilOperation DepthFail;

    /// <summary>
    /// The comparison operator used in the stencil test.
    /// </summary>
    public ComparisonKind Comparison;

    public StencilBehaviorDescription(StencilOperation fail, StencilOperation pass, StencilOperation depthFail, ComparisonKind comparison)
    {
        Fail = fail;
        Pass = pass;
        DepthFail = depthFail;
        Comparison = comparison;
    }

    public override bool Equals(object? obj)
    {
        return obj is StencilBehaviorDescription other && Equals(other);
    }

    public bool Equals(StencilBehaviorDescription other)
    {
        return Fail == other.Fail &&
               Pass == other.Pass &&
               DepthFail == other.DepthFail &&
               Comparison == other.Comparison;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Fail, Pass, DepthFail, Comparison);
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
