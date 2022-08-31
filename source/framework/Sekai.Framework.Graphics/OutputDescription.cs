// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct OutputDescription : IEquatable<OutputDescription>
{
    /// <summary>
    /// A description of the depth attachment.
    /// </summary>
    public OutputAttachmentDescription? DepthAttachment;

    /// <summary>
    /// An array of attachment descriptions, one for each color attachments.
    /// </summary>
    public OutputAttachmentDescription[] ColorAttachments;

    /// <summary>
    /// The nmumber of samples in each attachment.
    /// </summary>
    public TextureSampleCount SampleCount;

    public OutputDescription(OutputAttachmentDescription? depthAttachment, OutputAttachmentDescription[] colorAttachments, TextureSampleCount sampleCount)
    {
        DepthAttachment = depthAttachment;
        ColorAttachments = colorAttachments;
        SampleCount = sampleCount;
    }

    public override bool Equals(object? obj)
    {
        return obj is OutputDescription description && Equals(description);
    }

    public bool Equals(OutputDescription other)
    {
        return EqualityComparer<OutputAttachmentDescription?>.Default.Equals(DepthAttachment, other.DepthAttachment) &&
               Enumerable.SequenceEqual(ColorAttachments, other.ColorAttachments) &&
               SampleCount == other.SampleCount;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DepthAttachment, ColorAttachments, SampleCount);
    }

    public static bool operator ==(OutputDescription left, OutputDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(OutputDescription left, OutputDescription right)
    {
        return !(left == right);
    }
}
