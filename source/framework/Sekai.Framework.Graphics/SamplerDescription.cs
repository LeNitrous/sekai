// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct SamplerDescription : IEquatable<SamplerDescription>
{
    /// <summary>
    /// The address mode of the U (or S) coordinate.
    /// </summary>
    public SamplerAddressMode AddressModeU;

    /// <summary>
    /// The address mode of the V (or T) coordinate.
    /// </summary>
    public SamplerAddressMode AddressModeV;

    /// <summary>
    /// The address mode of the W (or R) coordinate.
    /// </summary>
    public SamplerAddressMode AddressModeW;

    /// <summary>
    /// The filter used in sampling.
    /// </summary>
    public SamplerFilter Filter;

    /// <summary>
    /// The comparison used when sampling. Use null if comparison sampling is not used.
    /// </summary>
    public ComparisonKind? Comparison;

    /// <summary>
    /// The maximum anisotropy of the filter. Used when <see cref="SamplerFilter.Anisotropic"/> is used.
    /// </summary>
    public uint MaximumAnisotropy;

    /// <summary>
    /// The minimum level of detail.
    /// </summary>
    public uint MinimumLod;

    /// <summary>
    /// The maximum level of detail.
    /// </summary>
    public uint MaximumLod;

    /// <summary>
    /// The level of detail bias.
    /// </summary>
    public int LodBias;

    /// <summary>
    /// The constant color that is sample when <see cref="SamplerAddressMode.Border"/> is used.
    /// </summary>
    public SamplerBorderColor Border;

    public SamplerDescription(SamplerAddressMode addressModeU, SamplerAddressMode addressModeV, SamplerAddressMode addressModeW, SamplerFilter filter, ComparisonKind? comparison, uint maximumAnisotropy, uint minimumLod, uint maximumLod, int lodBias, SamplerBorderColor border)
    {
        AddressModeU = addressModeU;
        AddressModeV = addressModeV;
        AddressModeW = addressModeW;
        Filter = filter;
        Comparison = comparison;
        MaximumAnisotropy = maximumAnisotropy;
        MinimumLod = minimumLod;
        MaximumLod = maximumLod;
        LodBias = lodBias;
        Border = border;
    }

    public override bool Equals(object? obj)
    {
        return obj is SamplerDescription other && Equals(other);
    }

    public bool Equals(SamplerDescription other)
    {
        return AddressModeU == other.AddressModeU &&
               AddressModeV == other.AddressModeV &&
               AddressModeW == other.AddressModeW &&
               Filter == other.Filter &&
               EqualityComparer<ComparisonKind?>.Default.Equals(Comparison, other.Comparison) &&
               MaximumAnisotropy == other.MaximumAnisotropy &&
               MinimumLod == other.MinimumLod &&
               MaximumLod == other.MaximumLod &&
               LodBias == other.LodBias &&
               Border == other.Border;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(AddressModeU);
        hash.Add(AddressModeV);
        hash.Add(AddressModeW);
        hash.Add(Filter);
        hash.Add(Comparison);
        hash.Add(MaximumAnisotropy);
        hash.Add(MinimumLod);
        hash.Add(MaximumLod);
        hash.Add(LodBias);
        hash.Add(Border);
        return hash.ToHashCode();
    }

    public static bool operator ==(SamplerDescription left, SamplerDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(SamplerDescription left, SamplerDescription right)
    {
        return !(left == right);
    }
}
