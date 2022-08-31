// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Sekai.Framework.Graphics;

public struct FramebufferAttachment : IEquatable<FramebufferAttachment>
{
    /// <summary>
    /// The target native texture which will be rendered to.
    /// </summary>
    public ITexture Target;

    /// <summary>
    /// The target array layer of the native texture.
    /// </summary>
    public uint ArrayLayer;

    /// <summary>
    /// The target mip level of the native texture.
    /// </summary>
    public uint MipLevel;

    public FramebufferAttachment(ITexture target, uint arrayLayer, uint mipLevel)
    {
        Target = target;
        MipLevel = mipLevel;
        ArrayLayer = arrayLayer;
    }

    public override bool Equals(object? obj)
    {
        return obj is FramebufferAttachment attachment && Equals(attachment);
    }

    public bool Equals(FramebufferAttachment other)
    {
        return EqualityComparer<ITexture>.Default.Equals(Target, other.Target) &&
               ArrayLayer == other.ArrayLayer &&
               MipLevel == other.MipLevel;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Target, ArrayLayer, MipLevel);
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
