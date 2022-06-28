// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using Sekai.Framework.Graphics.Textures;

namespace Sekai.Framework.Graphics;

public struct OutputInfo : IEquatable<OutputInfo>
{
    public readonly OutputAttachmentInfo? Depth;
    public readonly IReadOnlyList<OutputAttachmentInfo> Color;
    public readonly TextureSampleCount SampleCount;

    public OutputInfo(TextureSampleCount sampleCount, OutputAttachmentInfo? depth, params OutputAttachmentInfo[] color)
    {
        Depth = depth;
        Color = color;
        SampleCount = sampleCount;
    }

    public bool Equals(OutputInfo other)
    {
        return Depth == other.Depth
            && Color.SequenceEqual(other.Color, EqualityComparer<OutputAttachmentInfo>.Default)
            && SampleCount == other.SampleCount;
    }

    public override int GetHashCode() => HashCode.Combine(Depth, Color, SampleCount);

    public override bool Equals(object? obj) => obj is OutputInfo info && Equals(info);

    public static bool operator ==(OutputInfo left, OutputInfo right) => left.Equals(right);

    public static bool operator !=(OutputInfo left, OutputInfo right) => !(left == right);
}
