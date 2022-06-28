// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics.Textures;

namespace Sekai.Framework.Graphics.Buffers;

public struct FramebufferAttachment : IEquatable<FramebufferAttachment>
{
    public readonly Texture Target;
    public readonly int Layer;
    public readonly int MipLevel;
    public readonly FramebufferAttachmentKind Kind;

    public FramebufferAttachment(Texture target, FramebufferAttachmentKind kind, int layer, int mipLevel)
    {
        Kind = kind;
        Layer = layer;
        Target = target;
        MipLevel = mipLevel;
    }

    public bool Equals(FramebufferAttachment other) => ReferenceEquals(Target, other.Target) && Layer == other.Layer && MipLevel == other.MipLevel && Kind == other.Kind;

    public override bool Equals(object? obj) => obj is FramebufferAttachment attachment && Equals(attachment);
    public static bool operator ==(FramebufferAttachment left, FramebufferAttachment right) => left.Equals(right);
    public static bool operator !=(FramebufferAttachment left, FramebufferAttachment right) => !(left == right);

    public override int GetHashCode() => HashCode.Combine(Target, Layer, MipLevel, Kind);
}
