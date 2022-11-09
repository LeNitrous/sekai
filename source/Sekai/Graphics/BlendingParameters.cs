// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// Determines how color components should be blended to its destination.
/// </summary>
public struct BlendingParameters : IEquatable<BlendingParameters>
{
    /// <summary>
    /// Blending factor of the source color of the blend.
    /// </summary>
    public BlendingType SourceColor;

    /// <summary>
    /// Blending factor of the source alpha of the blend.
    /// </summary>
    public BlendingType SourceAlpha;

    /// <summary>
    /// Blending factor of the destination color of the blend.
    /// </summary>
    public BlendingType DestinationColor;

    /// <summary>
    /// Blending factor of the destination alpha of the blend.
    /// </summary>
    public BlendingType DestinationAlpha;

    /// <summary>
    /// The equation to use for the color components of the blend.
    /// </summary>
    public BlendingEquation ColorEquation;

    /// <summary>
    /// The equation to use for the alpha component of the blend.
    /// </summary>
    public BlendingEquation AlphaEquation;

    public static BlendingParameters None => new()
    {
        SourceColor = BlendingType.One,
        SourceAlpha = BlendingType.One,
        DestinationColor = BlendingType.Zero,
        DestinationAlpha = BlendingType.Zero,
        ColorEquation = BlendingEquation.Add,
        AlphaEquation = BlendingEquation.Add,
    };

    public static BlendingParameters Additive => new()
    {
        SourceColor = BlendingType.SourceAlpha,
        SourceAlpha = BlendingType.One,
        DestinationColor = BlendingType.One,
        DestinationAlpha = BlendingType.One,
        ColorEquation = BlendingEquation.Add,
        AlphaEquation = BlendingEquation.Add,
    };

    public readonly bool IsDisabled =>
        SourceColor == BlendingType.One &&
        SourceAlpha == BlendingType.One &&
        DestinationColor == BlendingType.Zero &&
        DestinationAlpha == BlendingType.Zero &&
        ColorEquation == BlendingEquation.Add &&
        AlphaEquation == BlendingEquation.Add;

    public override bool Equals(object? obj)
    {
        return obj is BlendingParameters other && Equals(other);
    }

    public bool Equals(BlendingParameters other)
    {
        return SourceColor == other.SourceColor &&
               SourceAlpha == other.SourceAlpha &&
               DestinationColor == other.DestinationColor &&
               DestinationAlpha == other.DestinationAlpha &&
               ColorEquation == other.ColorEquation &&
               AlphaEquation == other.AlphaEquation;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SourceColor, SourceAlpha, DestinationColor, DestinationAlpha, ColorEquation, AlphaEquation);
    }

    public static bool operator ==(BlendingParameters left, BlendingParameters right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlendingParameters left, BlendingParameters right)
    {
        return !(left == right);
    }
}
