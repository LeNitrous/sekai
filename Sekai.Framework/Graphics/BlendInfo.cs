// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct BlendInfo : IEquatable<BlendInfo>
{
    public bool AlphaToCoverage { get; set; }
    public Color Factor { get; set; }
    public IReadOnlyList<BlendAttachmentInfo> Attachments { get; set; }

    public bool Equals(BlendInfo other)
    {
        return AlphaToCoverage == other.AlphaToCoverage
            && Factor == other.Factor
            && Attachments.SequenceEqual(other.Attachments, EqualityComparer<BlendAttachmentInfo>.Default);
    }

    public override bool Equals(object? obj)
        => obj is BlendInfo info && Equals(info);

    public override int GetHashCode()
        => HashCode.Combine(AlphaToCoverage, Factor, Attachments);

    public static bool operator ==(BlendInfo left, BlendInfo right) => left.Equals(right);

    public static bool operator !=(BlendInfo left, BlendInfo right) => !(left == right);
}
