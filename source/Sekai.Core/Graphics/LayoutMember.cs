// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// An attribute used in fields to denote a given <see cref="ILayout"/> field is a layout member.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public sealed class LayoutMember : Attribute, IEquatable<LayoutMember>
{
    /// <summary>
    /// The layout member's name.
    /// </summary>
    public string Name { get; internal set; }

    /// <summary>
    /// The layout member's component count.
    /// </summary>
    public int Count { get; internal set; }

    /// <summary>
    /// Whether this layout member's value is normalized or not.
    /// </summary>
    public bool Normalized { get; }

    /// <summary>
    /// The layout member's value format.
    /// </summary>
    public LayoutMemberFormat Format { get; internal set; }

    /// <summary>
    /// Creates a layout member.
    /// </summary>
    /// <param name="name">The layout member's name.</param>
    /// <param name="count">The layout member's element count.</param>
    /// <param name="format">The layout member's format.</param>
    /// <param name="normalized">Whether this layout member has its values normalized.</param>
    public LayoutMember(string name, int count = 1, LayoutMemberFormat format = LayoutMemberFormat.Float, bool normalized = false)
    {
        Name = name;
        Count = count;
        Normalized = normalized;
        Format = format;
    }

    /// <summary>
    /// Creates a layout member whose name is implied from the field.
    /// </summary>
    /// <param name="count">The layout member's element count.</param>
    /// <param name="format">The layout member's format.</param>
    /// <param name="normalized">Whether this layout member has its values normalized.</param>
    public LayoutMember(int count = 1, LayoutMemberFormat format = LayoutMemberFormat.Float, bool normalized = false)
        : this(string.Empty, count, format, normalized)
    {
    }

    /// <summary>
    /// Creates a layout member whose name, element count, and type is implied from the field.
    /// </summary>
    /// <param name="normalized">Whether this layout member has its values normalized.</param>
    public LayoutMember(bool normalized = false)
        : this(string.Empty, int.MinValue, (LayoutMemberFormat)int.MinValue, normalized)
    {
    }

    /// <summary>
    /// Creates a layout member whose name, element count, and type is implied from the field and  whose values are implicity not normalized.
    /// </summary>
    public LayoutMember()
        : this(false)
    {
    }

    public bool Equals(LayoutMember? other)
    {
        return other is not null && Name == other.Name && Count == other.Count && Normalized == other.Normalized && Format == other.Format;
    }

    public override bool Equals(object? obj)
    {
        return obj is LayoutMember member && Equals(member);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Name, Count, Normalized, Format);
    }
}
