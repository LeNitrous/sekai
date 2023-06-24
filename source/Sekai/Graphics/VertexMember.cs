// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

public struct VertexMember : IEquatable<VertexMember>
{
    /// <summary>
    /// The member's component count.
    /// </summary>
    /// <remarks>
    /// For example, a <see cref="float"/> has 1 component. For a <see cref="System.Numerics.Vector2"/> there are 2 <see cref="float"/> components.
    /// </remarks>
    public int Count { get; }

    /// <summary>
    /// Whether the values should be normalized.
    /// </summary>
    public bool Normalized { get; }

    /// <summary>
    /// The vertex member's format.
    /// </summary>
    public VertexMemberFormat Format { get; }

    /// <summary>
    /// The vertex member's offset in bytes.
    /// </summary>
    public int Offset { get; set; }

    public VertexMember(int count = 1, bool normalized = false, VertexMemberFormat format = VertexMemberFormat.Float, int offset = 0)
    {
        Count = count;
        Normalized = normalized;
        Format = format;
        Offset = offset;
    }

    public readonly bool Equals(VertexMember other)
    {
        return Count == other.Count &&
               Normalized == other.Normalized &&
               Format == other.Format &&
               Offset == other.Offset;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is VertexMember other && Equals(other);

    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Count, Normalized, Format, Offset);
    }

    public static bool operator ==(VertexMember left, VertexMember right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VertexMember left, VertexMember right)
    {
        return !(left == right);
    }
}
