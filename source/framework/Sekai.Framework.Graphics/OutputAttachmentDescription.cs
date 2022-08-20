// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct OutputAttachmentDescription : IEquatable<OutputAttachmentDescription>
{
    /// <summary>
    /// The format of the <see cref="INativeTexture"/> attachment.
    /// </summary>
    public PixelFormat Format;

    public OutputAttachmentDescription(PixelFormat format)
    {
        Format = format;
    }

    public override bool Equals(object? obj)
    {
        return obj is OutputAttachmentDescription description && Equals(description);
    }

    public bool Equals(OutputAttachmentDescription other)
    {
        return Format == other.Format;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Format);
    }

    public static bool operator ==(OutputAttachmentDescription left, OutputAttachmentDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(OutputAttachmentDescription left, OutputAttachmentDescription right)
    {
        return !(left == right);
    }
}
