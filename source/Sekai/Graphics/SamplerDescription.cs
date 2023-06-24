// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;

namespace Sekai.Graphics;

/// <summary>
/// Describes a <see cref="Sampler"/>.
/// </summary>
public struct SamplerDescription : IEquatable<SamplerDescription>
{
    /// <summary>
    /// The filter to use.
    /// </summary>
    public TextureFilter Filter;

    /// <summary>
    /// The addressing mode for the U (or S) coordinate.
    /// </summary>
    public TextureAddress AddressU;

    /// <summary>
    /// The addressing mode for the V (or T) coordinate.
    /// </summary>
    public TextureAddress AddressV;

    /// <summary>
    /// The addressing mode for the W (or R) coordinate.
    /// </summary>
    public TextureAddress AddressW;

    /// <summary>
    /// The maximum number of anisotropic levels when <see cref="TextureFilter.Anisotropic"/> is used.
    /// </summary>
    public int MaxAnisotropy;

    /// <summary>
    /// The border color when <see cref="TextureAddress.ClampToBorder"/> is used.
    /// </summary>
    public Color BorderColor;

    /// <summary>
    /// The minimum level of detail.
    /// </summary>
    public float MinimumLOD;

    /// <summary>
    /// The maximum level of detail.
    /// </summary>
    public float MaximumLOD;

    /// <summary>
    /// The bias in level of detail.
    /// </summary>
    public float LODBias;

    public SamplerDescription(TextureFilter filter, TextureAddress addressU, TextureAddress addressV, TextureAddress addressW, int maxAnisotropy, Color borderColor, float minLOD, float maxLOD, float lodBias)
    {
        Filter = filter;
        AddressU = addressU;
        AddressV = addressV;
        AddressW = addressW;
        MaxAnisotropy = maxAnisotropy;
        BorderColor = borderColor;
        MinimumLOD = minLOD;
        MaximumLOD = maxLOD;
        LODBias = lodBias;
    }

    public readonly bool Equals(SamplerDescription other)
    {
        return Filter == other.Filter &&
               AddressU == other.AddressU &&
               AddressV == other.AddressV &&
               AddressW == other.AddressW &&
               MaxAnisotropy == other.MaxAnisotropy &&
               BorderColor.Equals(other.BorderColor) &&
               MinimumLOD == other.MinimumLOD &&
               MaximumLOD == other.MaximumLOD;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is SamplerDescription other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Filter, AddressU, AddressV, AddressW, MaxAnisotropy, BorderColor, MinimumLOD, MaximumLOD);
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
