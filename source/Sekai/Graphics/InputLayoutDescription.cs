// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sekai.Graphics;

public readonly struct InputLayoutDescription : IEquatable<InputLayoutDescription>
{
    public IReadOnlyList<InputLayoutMember> Members { get; }

    public InputLayoutDescription(params InputLayoutMember[] members)
    {
        Members = members;
    }

    public bool Equals(InputLayoutDescription other)
    {
        return Members.SequenceEqual(other.Members);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is InputLayoutDescription input && Equals(input);
    }

    public override int GetHashCode()
    {
        return ((IStructuralEquatable)Members).GetHashCode(EqualityComparer<InputLayoutMember>.Default);
    }

    public static bool operator ==(InputLayoutDescription left, InputLayoutDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InputLayoutDescription left, InputLayoutDescription right)
    {
        return !(left == right);
    }
}

public readonly struct InputLayoutMember : IEquatable<InputLayoutMember>
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
    /// The layout's format.
    /// </summary>
    public InputLayoutFormat Format { get; }

    public InputLayoutMember(int count = 1, bool normalized = false, InputLayoutFormat format = InputLayoutFormat.Float)
    {
        Count = count;
        Format = format;
        Normalized = normalized;
    }

    public readonly bool Equals(InputLayoutMember other)
    {
        return Count == other.Count &&
               Format == other.Format &&
               Normalized == other.Normalized;
    }

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj is InputLayoutMember other && Equals(other);

    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(Count, Normalized, Format);
    }

    public static bool operator ==(InputLayoutMember left, InputLayoutMember right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(InputLayoutMember left, InputLayoutMember right)
    {
        return !(left == right);
    }
}
