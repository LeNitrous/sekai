// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Sekai.Graphics;

/// <summary>
/// Defines a vertex's layout.
/// </summary>
public readonly struct VertexLayout : IEquatable<VertexLayout>
{
    /// <summary>
    /// The layout's stride.
    /// </summary>
    public uint Stride { get; }

    /// <summary>
    /// The layout's members.
    /// </summary>
    public IReadOnlyList<VertexMember> Members { get; }

    public VertexLayout(params VertexMember[] members)
    {
        Stride = (uint)members.Aggregate(0, static (acc, cur) => acc + (cur.Format.SizeOfFormat() * cur.Count));
        Members = members;
    }

    public bool Equals(VertexLayout other)
    {
        return Members?.SequenceEqual(other.Members, EqualityComparer<VertexMember>.Default) == true;
    }

    public override bool Equals(object? obj)
    {
        return obj is VertexLayout vertexLayout && Equals(vertexLayout);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Members);
    }

    public static bool operator ==(VertexLayout left, VertexLayout right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VertexLayout left, VertexLayout right)
    {
        return !(left == right);
    }
}
