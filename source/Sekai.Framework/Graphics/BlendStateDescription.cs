// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Describes a <see cref="BlendState"/>.
/// </summary>
public struct BlendStateDescription : IEquatable<BlendStateDescription>
{
    /// <summary>
    /// Whether blending is enabled or not.
    /// </summary>
    public bool Enabled;

    /// <summary>
    /// The source color blending.
    /// </summary>
    public BlendType SourceColor;

    /// <summary>
    /// The destination color blending.
    /// </summary>
    public BlendType DestinationColor;

    /// <summary>
    /// The operation to perform between the <see cref="SourceColor"/> and <see cref="DesinationColor"/>.
    /// </summary>
    public BlendOperation ColorOperation;

    /// <summary>
    /// The source alpha blending.
    /// </summary>
    public BlendType SourceAlpha;

    /// <summary>
    /// The destination alpha blending.
    /// </summary>
    public BlendType DestinationAlpha;

    /// <summary>
    /// The operation to perform between the <see cref="SourceAlpha"/> and <see cref="DesinationAlpha"/>.
    /// </summary>
    public BlendOperation AlphaOperation;

    /// <summary>
    /// The color mask.
    /// </summary>
    public ColorWriteMask WriteMask;

    public BlendStateDescription(bool enabled, BlendType sourceColor, BlendType destinationColor, BlendOperation colorOperation, BlendType sourceAlpha, BlendType destinationAlpha, BlendOperation alphaOperation, ColorWriteMask writeMask)
    {
        Enabled = enabled;
        SourceColor = sourceColor;
        DestinationColor = destinationColor;
        ColorOperation = colorOperation;
        SourceAlpha = sourceAlpha;
        DestinationAlpha = destinationAlpha;
        AlphaOperation = alphaOperation;
        WriteMask = writeMask;
    }

    public BlendStateDescription(BlendType sourceColor, BlendType destinationColor, BlendType sourceAlpha, BlendType destinationAlpha)
        : this(true, sourceColor, destinationColor, BlendOperation.Add, sourceAlpha, destinationAlpha, BlendOperation.Add, ColorWriteMask.All)
    {
    }

    public BlendStateDescription(BlendType source, BlendType destination)
        : this(source, destination, source, destination)
    {
    }

    public bool Equals(BlendStateDescription other)
    {
        return Enabled == other.Enabled &&
               SourceColor == other.SourceColor &&
               DestinationColor == other.DestinationColor &&
               ColorOperation == other.ColorOperation &&
               SourceAlpha == other.SourceAlpha &&
               DestinationAlpha == other.DestinationAlpha &&
               AlphaOperation == other.AlphaOperation &&
               WriteMask == other.WriteMask;
    }

    public override bool Equals(object? obj)
    {
        return obj is BlendStateDescription other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Enabled, SourceColor, DestinationColor, ColorOperation, SourceAlpha, DestinationAlpha, AlphaOperation, WriteMask);
    }

    /// <summary>
    /// Disabled blending.
    /// </summary>
    public static readonly BlendStateDescription Disabled = new(false, BlendType.One, BlendType.Zero, BlendOperation.Add, BlendType.One, BlendType.Zero, BlendOperation.Add, ColorWriteMask.All);

    /// <summary>
    /// Non-premultiplied alpha blending.
    /// </summary>
    public static readonly BlendStateDescription NonPremultiplied = new(BlendType.SourceAlpha, BlendType.OneMinusSourceAlpha);

    /// <summary>
    /// Alpha blending.
    /// </summary>
    public static readonly BlendStateDescription AlphaBlend = new(BlendType.One, BlendType.OneMinusSourceAlpha);

    /// <summary>
    /// Additive blending.
    /// </summary>
    public static readonly BlendStateDescription Additive = new(BlendType.SourceAlpha, BlendType.One);

    /// <summary>
    /// Opaque blending.
    /// </summary>
    public static readonly BlendStateDescription Opaque = new(BlendType.One, BlendType.Zero);

    public static bool operator ==(BlendStateDescription left, BlendStateDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BlendStateDescription left, BlendStateDescription right)
    {
        return !(left == right);
    }
}
