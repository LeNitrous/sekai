// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Graphics;

/// <summary>
/// Describes a part of a <see cref="Framebuffer"/>.
/// </summary>
public struct FramebufferAttachment : IEquatable<FramebufferAttachment>
{
    /// <summary>
    /// The texture used to attach.
    /// </summary>
    public Texture Texture;

    /// <summary>
    /// The array layer to attach to.
    /// </summary>
    public int Layer;

    /// <summary>
    /// The mip level to attach to.
    /// </summary>
    public int Level;

    public FramebufferAttachment(Texture texture, int layer, int level)
    {
        Level = level;
        Layer = layer;
        Texture = texture;
    }

    public readonly bool Equals(FramebufferAttachment other)
    {
        return EqualityComparer<Texture>.Default.Equals(Texture, other.Texture) && Layer.Equals(other.Layer) && Level.Equals(other.Level);
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is FramebufferAttachment other && Equals(other);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Texture, Layer, Level);
    }

    public static bool operator ==(FramebufferAttachment left, FramebufferAttachment right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(FramebufferAttachment left, FramebufferAttachment right)
    {
        return !(left == right);
    }
}
