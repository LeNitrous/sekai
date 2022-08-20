// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct BlendAttachmentDescription : IEquatable<BlendAttachmentDescription>
{
    /// <summary>
    /// Whether blending is enabled for this attachment.
    /// </summary>
    public bool Enabled;

    /// <summary>
    /// Controls which components of the color will be written to the <see cref="IFramebuffer"/>.
    /// </summary>
    public ColorWriteMask? ColorWriteMask;

    /// <summary>
    /// Controls the source color's influence on the blend result.
    /// </summary>
    public BlendFactor SourceColor;

    /// <summary>
    /// Controls the source alpha's influence on the blend result.
    /// </summary>
    public BlendFactor SourceAlpha;

    /// <summary>
    /// Controls the destination color's influence on the blend result.
    /// </summary>
    public BlendFactor DestinationColor;

    /// <summary>
    /// Controls the destination alpha's influence on the blend result.
    /// </summary>
    public BlendFactor DestinationAlpha;

    /// <summary>
    /// Controls the function used to combine the source and destination color factors.
    /// </summary>
    public BlendFunction Color;

    /// <summary>
    /// Controls the function used to combine the source and destination alpha factors.
    /// </summary>
    public BlendFunction Alpha;

    public BlendAttachmentDescription(bool enabled, ColorWriteMask? colorWriteMask, BlendFactor sourceColor, BlendFactor sourceAlpha, BlendFactor destinationColor, BlendFactor destinationAlpha, BlendFunction color, BlendFunction alpha)
    {
        Enabled = enabled;
        ColorWriteMask = colorWriteMask;
        SourceColor = sourceColor;
        SourceAlpha = sourceAlpha;
        DestinationColor = destinationColor;
        DestinationAlpha = destinationAlpha;
        Color = color;
        Alpha = alpha;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlendAttachmentDescription other && Equals(other);
    }

    public bool Equals(BlendAttachmentDescription other)
    {
        return Enabled == other.Enabled &&
               ColorWriteMask == other.ColorWriteMask &&
               SourceColor == other.SourceColor &&
               SourceAlpha == other.SourceAlpha &&
               DestinationColor == other.DestinationColor &&
               DestinationAlpha == other.DestinationAlpha &&
               Color == other.Color &&
               Alpha == other.Alpha;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Enabled, ColorWriteMask, SourceColor, SourceAlpha, DestinationColor, DestinationAlpha, Color, Alpha);
    }

    public static bool operator ==(BlendAttachmentDescription left, BlendAttachmentDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlendAttachmentDescription left, BlendAttachmentDescription right)
    {
        return !(left == right);
    }
}
