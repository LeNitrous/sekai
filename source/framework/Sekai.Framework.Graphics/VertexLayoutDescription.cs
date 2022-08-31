// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Linq;

namespace Sekai.Framework.Graphics;

public struct VertexLayoutDescription : IEquatable<VertexLayoutDescription>
{
    /// <summary>
    /// The number of bytes between successive elements in the <see cref="IBuffer"/>.
    /// </summary>
    public uint Stride;

    /// <summary>
    /// Descriptions of each element in this layout.
    /// </summary>
    public VertexElementDescription[] Elements;

    /// <summary>
    /// A value controlling how often data for instances is advanced for this layout.
    /// </summary>
    public uint InstanceStepRate;

    public VertexLayoutDescription(uint stride, VertexElementDescription[] elements)
    {
        Stride = stride;
        Elements = elements;
        InstanceStepRate = 0;
    }

    public VertexLayoutDescription(uint stride, VertexElementDescription[] elements, uint instanceStepRate)
    {
        Stride = stride;
        Elements = elements;
        InstanceStepRate = instanceStepRate;
    }

    public override bool Equals(object? obj)
    {
        return obj is VertexLayoutDescription description && Equals(description);
    }

    public bool Equals(VertexLayoutDescription other)
    {
        return Stride == other.Stride &&
               Enumerable.SequenceEqual(Elements, other.Elements) &&
               InstanceStepRate == other.InstanceStepRate;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Stride, Elements, InstanceStepRate);
    }

    public static bool operator ==(VertexLayoutDescription left, VertexLayoutDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VertexLayoutDescription left, VertexLayoutDescription right)
    {
        return !(left == right);
    }
}
