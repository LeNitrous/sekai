// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct StencilBehavior : IEquatable<StencilBehavior>
{
    public StencilOperation Fail { get; set; }
    public StencilOperation Pass { get; set; }
    public StencilOperation DepthFail { get; set; }
    public ComparisonKind Comparison { get; set; }

    public bool Equals(StencilBehavior other)
    {
        return Fail == other.Fail
            && Pass == other.Pass
            && DepthFail == other.DepthFail
            && Comparison == other.Comparison;
    }

    public override bool Equals(object? obj)
        => obj is StencilBehavior behavior && Equals(behavior);

    public override int GetHashCode()
        => HashCode.Combine(Fail, Pass, DepthFail, Comparison);

    public static bool operator ==(StencilBehavior left, StencilBehavior right) => left.Equals(right);

    public static bool operator !=(StencilBehavior left, StencilBehavior right) => !(left == right);
}
