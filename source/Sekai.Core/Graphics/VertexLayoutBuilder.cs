// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Extensions;
using Veldrid;

namespace Sekai.Graphics;

/// <summary>
/// A helper that builds a <see cref="VertexLayout"/>.
/// </summary>
public class VertexLayoutBuilder
{
    private readonly HashSet<VertexMember> members = new();

    /// <summary>
    /// Adds a type as a layout member.
    /// </summary>
    /// <typeparam name="T">The member type.</typeparam>
    /// <param name="name">The member name.</param>
    /// <param name="normalized">Whether this member's value is normalized.</param>
    public VertexLayoutBuilder Add<T>(string name, bool normalized = false)
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
    public VertexLayoutBuilder Add(Type type, string name, bool normalized = false)
    {
        if (!formatMap.TryGetValue(type, out var format))
            throw new NotSupportedException($"The type {type} is not a supported layout member type.");

        if (!countMap.TryGetValue(type, out int count))
            count = 1;

        return Add(new VertexMember(name, count, format, normalized));
    }

    /// <summary>
    /// Adds a layout member.
    /// </summary>
    /// <param name="name">The member name.</param>
    /// <param name="count">The member's component count.</param>
    /// <param name="format">The member's component format.</param>
    /// <param name="normalized">Whether this member's value is normalized.</param>
    public VertexLayoutBuilder Add(string name, int count, VertexMemberFormat format, bool normalized = false)
    {
        return Add(new VertexMember(name, count, format, normalized));
    }

    /// <summary>
    /// Appends another <see cref="VertexLayoutBuilder"/> to itself.
    /// </summary>
    /// <param name="other">The other <see cref="VertexLayoutBuilder"/>.</param>
    public VertexLayoutBuilder Add(VertexLayoutBuilder other)
    {
        foreach (var member in other.members)
        {
            Add(member);
        }

        return this;
    }

    /// <summary>
    /// Appends a <see cref="VertexMember"/> to itself.
    /// </summary>
    /// <param name="member">The layout member to append.</param>
    internal VertexLayoutBuilder Add(VertexMember member)
    {
        members.Add(member);
        return this;
    }

    /// <summary>
    /// Builds this <see cref="VertexLayoutBuilder"/> as a <see cref="VertexLayout"/>.
    /// </summary>
    /// <returns>The compiled <see cref="VertexLayout"/>.</returns>
    public VertexLayout Build()
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

        return new VertexLayout(new((uint)stride, elements));
    }

    private static readonly Dictionary<Type, VertexMemberFormat> formatMap = new()
    {
        { typeof(int), VertexMemberFormat.Int },
        { typeof(uint), VertexMemberFormat.UnsignedInt },
        { typeof(byte), VertexMemberFormat.UnsignedByte },
        { typeof(sbyte), VertexMemberFormat.Byte },
        { typeof(short), VertexMemberFormat.Short },
        { typeof(ushort), VertexMemberFormat.UnsignedShort },
        { typeof(Half), VertexMemberFormat.Half },
        { typeof(float), VertexMemberFormat.Float },
        { typeof(Vector2), VertexMemberFormat.Float },
        { typeof(Vector3), VertexMemberFormat.Float },
        { typeof(Vector4), VertexMemberFormat.Float },
        { typeof(Matrix4x4), VertexMemberFormat.Float },
    };

    private static readonly Dictionary<Type, int> countMap = new()
    {
        { typeof(Vector2), 2 },
        { typeof(Vector3), 3 },
        { typeof(Vector4), 4 },
        { typeof(Matrix4x4), 16 },
    };
}
