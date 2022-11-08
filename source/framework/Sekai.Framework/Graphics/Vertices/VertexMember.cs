// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Vertices;

[AttributeUsage(AttributeTargets.Field)]
public sealed class VertexMember : Attribute, IEquatable<VertexMember>
{
    /// <summary>
    /// The member's name.
    /// </summary>
    public string? Name { get; internal set; }

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
    /// The member's format.
    /// </summary>
    public VertexMemberFormat Format { get; }

    /// <summary>
    /// The member's offset in bytes.
    /// </summary>
    public int Offset { get; internal set; }

    public VertexMember(string? name = null, int count = 1, VertexMemberFormat format = VertexMemberFormat.Float, bool normalized = false)
    {
        Name = name;
        Count = count;
        Format = format;
        Normalized = normalized;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VertexMember);

    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), TypeId, Name, Count, Normalized, Format);
    }

    public bool Equals(VertexMember? other)
    {
        return other is not null && Name == other.Name && Count == other.Count && Normalized == other.Normalized && Format == other.Format;
    }
}
