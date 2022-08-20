// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;

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
    public NativeTextureSampleCount SampleCount;

    public OutputDescription(OutputAttachmentDescription? depthAttachment, OutputAttachmentDescription[] colorAttachments, NativeTextureSampleCount sampleCount)
    {
        DepthAttachment = depthAttachment;
        ColorAttachments = colorAttachments;
        SampleCount = sampleCount;
    }

    public override bool Equals(object? obj)
    {
        return obj is OutputDescription description &&
               EqualityComparer<OutputAttachmentDescription?>.Default.Equals(DepthAttachment, description.DepthAttachment) &&
               EqualityComparer<OutputAttachmentDescription[]>.Default.Equals(ColorAttachments, description.ColorAttachments) &&
               SampleCount == description.SampleCount;
    }

    public bool Equals(OutputDescription other)
    {
        throw new NotImplementedException();
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
