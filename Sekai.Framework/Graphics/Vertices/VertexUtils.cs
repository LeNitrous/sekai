// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sekai.Framework.Extensions;
using Veldrid;

namespace Sekai.Framework.Graphics.Vertices;

internal static class VertexUtils<T>
{
    public static readonly int Stride = Unsafe.SizeOf<T>();
    public static readonly VertexLayoutDescription Layout;

    static VertexUtils()
    {
        Layout = new(getMembersRecursive(typeof(T)));
    }

    private static VertexElementDescription[] getMembersRecursive(Type type, List<VertexElementDescription>? elements = null)
    {
        elements ??= new();

        foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (typeof(IVertex).IsAssignableFrom(field.FieldType))
            {
                getMembersRecursive(type, elements);
            }
            else if (field.IsDefined(typeof(VertexMemberAttribute), true))
            {
                var attrib = field.GetCustomAttribute<VertexMemberAttribute>(true);

                if (attrib != null)
                    elements.Add(new(attrib.Name ?? field.Name, VertexElementSemantic.TextureCoordinate, attrib.Format.ToVeldrid()));
            }
        }

        return elements.ToArray();
    }
}
