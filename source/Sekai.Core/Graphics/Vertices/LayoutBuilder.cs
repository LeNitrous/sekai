// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Extensions;
using Veldrid;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// A helper that builds a <see cref="Layout"/>.
/// </summary>
public class LayoutBuilder
{
    private readonly HashSet<LayoutMember> members = new();

    /// <summary>
    /// Adds a type as a layout member.
    /// </summary>
    /// <typeparam name="T">The member type.</typeparam>
    /// <param name="name">The member name.</param>
    /// <param name="normalized">Whether this member's value is normalized.</param>
    public LayoutBuilder Add<T>(string name, bool normalized = false)
        where T : unmanaged
    {
        return Add(typeof(T), name, normalized);
    }

    /// <summary>
    /// Adds a type as a layout member.
    /// </summary>
    /// <param name="type">The member type.</param>
    /// <param name="name">The member name.</param>
    /// <param name="normalized">Whether this member's value is normalized.</param>
    public LayoutBuilder Add(Type type, string name, bool normalized = false)
    {
        if (!formatMap.TryGetValue(type, out var format))
            throw new NotSupportedException($"The type {type} is not a supported layout member type.");

        if (!countMap.TryGetValue(type, out int count))
            count = 1;

        return Add(new LayoutMember(name, count, format, normalized));
    }

    /// <summary>
    /// Adds a layout member.
    /// </summary>
    /// <param name="name">The member name.</param>
    /// <param name="count">The member's component count.</param>
    /// <param name="format">The member's component format.</param>
    /// <param name="normalized">Whether this member's value is normalized.</param>
    public LayoutBuilder Add(string name, int count, LayoutMemberFormat format, bool normalized = false)
    {
        return Add(new LayoutMember(name, count, format, normalized));
    }

    /// <summary>
    /// Appends another <see cref="LayoutBuilder"/> to itself.
    /// </summary>
    /// <param name="other">The other <see cref="LayoutBuilder"/>.</param>
    public LayoutBuilder Add(LayoutBuilder other)
    {
        foreach (var member in other.members)
        {
            Add(member);
        }

        return this;
    }

    /// <summary>
    /// Appends a <see cref="LayoutMember"/> to itself.
    /// </summary>
    /// <param name="member">The layout member to append.</param>
    internal LayoutBuilder Add(LayoutMember member)
    {
        members.Add(member);
        return this;
    }

    /// <summary>
    /// Builds this <see cref="LayoutBuilder"/> as a <see cref="Layout"/>.
    /// </summary>
    /// <returns>The compiled <see cref="Layout"/>.</returns>
    public Layout Build()
    {
        int index = 0;
        int stride = 0;
        var elements = new VertexElementDescription[members.Count];

        foreach (var member in members)
        {
            elements[index] = new(member.Name, VertexElementSemantic.TextureCoordinate, member.Format.AsVertexFormat(member.Count, member.Normalized), (uint)stride);

            index++;
            stride += member.Format.SizeOfFormat() * member.Count;
        }

        return new Layout(new((uint)stride, elements));
    }

    private static readonly Dictionary<Type, LayoutMemberFormat> formatMap = new()
    {
        { typeof(int), LayoutMemberFormat.Int },
        { typeof(uint), LayoutMemberFormat.UnsignedInt },
        { typeof(byte), LayoutMemberFormat.UnsignedByte },
        { typeof(sbyte), LayoutMemberFormat.Byte },
        { typeof(short), LayoutMemberFormat.Short },
        { typeof(ushort), LayoutMemberFormat.UnsignedShort },
        { typeof(Half), LayoutMemberFormat.Half },
        { typeof(float), LayoutMemberFormat.Float },
        { typeof(Vector2), LayoutMemberFormat.Float },
        { typeof(Vector3), LayoutMemberFormat.Float },
        { typeof(Vector4), LayoutMemberFormat.Float },
        { typeof(Matrix4x4), LayoutMemberFormat.Float },
    };

    private static readonly Dictionary<Type, int> countMap = new()
    {
        { typeof(Vector2), 2 },
        { typeof(Vector3), 3 },
        { typeof(Vector4), 4 },
        { typeof(Matrix4x4), 16 },
    };
}
