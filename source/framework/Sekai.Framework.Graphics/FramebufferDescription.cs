// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct FramebufferDescription : IEquatable<FramebufferDescription>
{
    /// <summary>
    /// The depth texture.
    /// </summary>
    public FramebufferAttachment? DepthTarget;

    /// <summary>
    /// An array of color textures.
    /// </summary>
    public FramebufferAttachment[] ColorTargets;

    public FramebufferDescription(FramebufferAttachment? depthTarget, FramebufferAttachment[] colorTargets)
    {
        DepthTarget = depthTarget;
        ColorTargets = colorTargets;
    }

    public override bool Equals(object? obj)
    {
        return obj is FramebufferDescription description && Equals(description);
    }

    public bool Equals(FramebufferDescription other)
    {
        return EqualityComparer<FramebufferAttachment?>.Default.Equals(DepthTarget, other.DepthTarget) &&
               Enumerable.SequenceEqual(ColorTargets, other.ColorTargets);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DepthTarget, ColorTargets);
    }

    public static bool operator ==(FramebufferDescription left, FramebufferDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FramebufferDescription left, FramebufferDescription right)
    {
        return !(left == right);
    }
}
