// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct BlendStateDescription : IEquatable<BlendStateDescription>
{
    /// <summary>
    /// A constant blend color used in <see cref="BlendFactor.BlendFactor"/> and <see cref="BlendFactor.InverseBlendFactor"/>.
    /// </summary>
    public Color4 Factor;

    /// <summary>
    /// An array of attachemnts describing how blending is performed for each color target in the <see cref="IPipeline"/>
    /// </summary>
    public BlendAttachmentDescription[] Attachments;

    /// <summary>
    /// Enables alpha-to-coverage, which causes a fragment's alpha value to be used when determining multi-sample coverage.
    /// </summary>
    public bool AlphaToConvergeEnabled;

    public BlendStateDescription(Color4 factor, BlendAttachmentDescription[] attachments, bool alphaToConvergeEnabled)
    {
        Factor = factor;
        Attachments = attachments;
        AlphaToConvergeEnabled = alphaToConvergeEnabled;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlendStateDescription other && Equals(other);
    }

    public bool Equals(BlendStateDescription other)
    {
        return Factor.Equals(other.Factor) &&
               Enumerable.SequenceEqual(Attachments, other.Attachments) &&
               AlphaToConvergeEnabled == other.AlphaToConvergeEnabled;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Factor, Attachments, AlphaToConvergeEnabled);
    }

    public static bool operator ==(BlendStateDescription left, BlendStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlendStateDescription left, BlendStateDescription right)
    {
        return !(left == right);
    }
}
