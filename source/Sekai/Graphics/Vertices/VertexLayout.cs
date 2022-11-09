// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sekai.Mathematics;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// Describes a vertex layout.
/// </summary>
public class VertexLayout : IVertexLayout
{
    public int Stride { get; private set; }
    public IReadOnlyList<VertexMember> Members => members;
    private readonly List<VertexMember> members = new();

    /// <summary>
    /// Appends a layout to this layout.
    /// </summary>
    public void Add(IVertexLayout layout)
    {
        foreach (var member in layout.Members)
        {
            member.Offset = Stride;
            members.Add(member);
            Stride += member.Format.SizeOfFormat();
        }
    }

    /// <summary>
    /// Adds a member for this vertex layout.
    /// </summary>
    public void Add(VertexMember member)
    {
        if (members.Contains(member))
            return;

        member.Offset = Stride;
        members.Add(member);
        Stride += member.Format.SizeOfFormat();
    }

    /// <summary>
    /// Adds a known type to the vertex layout.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="name">The member's name.</param>
    /// <param name="normalized">Whether values should be normalized.</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Add(Type type, string name, bool normalized = true)
    {
        if (!formatMap.TryGetValue(type, out var format))
            throw new NotSupportedException();

        if (!countMap.TryGetValue(type, out int count))
            count = 1;

        Add(new VertexMember(name, count, format, normalized));
    }

    /// <summary>
    /// Adds a known type to the vertex layout.
    /// </summary>
    /// <typeparam name="T">The type.</typeparam>
    /// <param name="name">The member's name.</param>
    /// <param name="normalized">Whether values should be normalized.</param>
    /// <exception cref="NotSupportedException"></exception>
    public void Add<T>(string name, bool normalized = true)
    {
        Add(typeof(T), name, normalized);
    }

    /// <summary>
    /// Builds a vertex layout from a given type.
    /// </summary>
    /// <typeparam name="T">A type that impelements <see cref="IVertex"/>.</typeparam>
    /// <returns>A vertex layout from a type.</returns>
    public static IVertexLayout From<T>()
        where T : unmanaged, IVertex
    {
        return From(typeof(T));
    }

    internal static IVertexLayout From(Type type)
    {
        if (!type.IsAssignableTo(typeof(IVertex)))
            throw new ArgumentException(@$"""{type}"" does not implement {nameof(IVertex)}.");

        var layout = new VertexLayout();
        buildLayout(type, layout);

        static void buildLayout(Type t, VertexLayout layout)
        {
            foreach (var field in t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType.IsAssignableTo(typeof(IVertex)))
                {
                    buildLayout(field.FieldType, layout);
                }
                else
                {
                    var attr = field.GetCustomAttribute<VertexMember>();

                    if (attr is null)
                        continue;

                    if (string.IsNullOrEmpty(attr.Name))
                        attr.Name = field.Name;

                    layout.Add(attr);
                }
            }
        }

        return layout;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IVertexLayout);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Members, members);
    }

    public bool Equals(IVertexLayout? other)
    {
        return other is not null && Enumerable.SequenceEqual(members, other.Members, EqualityComparer<VertexMember>.Default);
    }

    private static readonly Dictionary<Type, VertexMemberFormat> formatMap = new()
    {
        { typeof(int), VertexMemberFormat.Int },
        { typeof(uint), VertexMemberFormat.UnsignedInt },
        { typeof(byte), VertexMemberFormat.UnsignedByte },
        { typeof(sbyte), VertexMemberFormat.Byte },
        { typeof(short), VertexMemberFormat.Short },
        { typeof(ushort), VertexMemberFormat.UnsignedShort },
        { typeof(float), VertexMemberFormat.Float },
        { typeof(double), VertexMemberFormat.Double },
        { typeof(Vector2), VertexMemberFormat.Float },
        { typeof(Vector3), VertexMemberFormat.Float },
        { typeof(Vector4), VertexMemberFormat.Float },
        { typeof(Color4), VertexMemberFormat.Float },
        { typeof(Matrix), VertexMemberFormat.Float },
    };

    private static readonly Dictionary<Type, int> countMap = new()
    {
        { typeof(Vector2), 2 },
        { typeof(Vector3), 3 },
        { typeof(Vector4), 4 },
        { typeof(Color4), 4 },
        { typeof(Matrix), 16 },
    };
}
