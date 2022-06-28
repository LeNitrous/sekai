// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct BlendAttachmentInfo : IEquatable<BlendAttachmentInfo>
{
    public bool Enabled { get; set; }
    public BlendFactor SourceColor { get; set; }
    public BlendFactor SourceAlpha { get; set; }
    public BlendFactor DestinationColor { get; set; }
    public BlendFactor DestinationAlpha { get; set; }
    public BlendFunction Color { get; set; }
    public BlendFunction Alpha { get; set; }

    public bool Equals(BlendAttachmentInfo other)
    {
        return Enabled == other.Enabled
            && SourceColor == other.SourceColor
            && SourceAlpha == other.SourceAlpha
            && DestinationColor == other.DestinationColor
            && DestinationAlpha == other.DestinationAlpha
            && Color == other.Color
            && Alpha == other.Alpha;
    }

    public override int GetHashCode()
        => HashCode.Combine(Enabled, SourceColor, SourceAlpha, DestinationColor, DestinationAlpha, Color, Alpha);

    public override bool Equals(object? obj)
        => obj is BlendAttachmentInfo info && Equals(info);

    public static bool operator ==(BlendAttachmentInfo left, BlendAttachmentInfo right) => left.Equals(right);

    public static bool operator !=(BlendAttachmentInfo left, BlendAttachmentInfo right) => !(left == right);
}
