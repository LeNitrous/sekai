// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct OutputAttachmentInfo : IEquatable<OutputAttachmentInfo>
{
    public readonly PixelFormat Format;

    public OutputAttachmentInfo(PixelFormat format)
    {
        Format = format;
    }

    public bool Equals(OutputAttachmentInfo other) => other.Format == Format;

    public override int GetHashCode() => HashCode.Combine(Format);

    public override bool Equals(object? obj) => obj is OutputAttachmentInfo info && Equals(info);

    public static bool operator ==(OutputAttachmentInfo left, OutputAttachmentInfo right) => left.Equals(right);

    public static bool operator !=(OutputAttachmentInfo left, OutputAttachmentInfo right) => !(left == right);
}
