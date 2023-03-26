// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Veldrid;

namespace Sekai.Graphics.Vertices;

/// <summary>
/// Represents a compiled <see cref="IVertex"/>.
/// </summary>
public class Layout
{
    /// <summary>
    /// A read-only list of defined layout members.
    /// </summary>
    internal IReadOnlyList<VertexElementDescription> Members => descriptor.Elements;

    private readonly VertexLayoutDescription descriptor;

    internal Layout(VertexLayoutDescription descriptor)
    {
        this.descriptor = descriptor;
    }

    /// <summary>
    /// Creates a <see cref="Layout"/> from an <see cref="IVertex"/>.
    /// </summary>
    /// <typeparam name="T">The <see cref="IVertex"/> to construct from.</typeparam>
    public static Layout Create<T>()
        where T : unmanaged, IVertex
    {
        var builder = new LayoutBuilder();

        build(typeof(T), builder);

        return builder.Build();

        static void build(Type type, LayoutBuilder builder)
        {
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (field.FieldType.IsAssignableTo(typeof(IVertex)))
                {
                    build(field.FieldType, builder);
                }
                else
                {
                    var attr = field.GetCustomAttribute<LayoutMember>();

                    if (attr is null)
                    {
                        continue;
                    }

                    if (string.IsNullOrEmpty(attr.Name))
                    {
                        attr.Name = field.Name;
                    }

                    if (attr.Count == int.MinValue && attr.Format == (LayoutMemberFormat)int.MinValue)
                    {
                        builder.Add(field.FieldType, attr.Name, attr.Normalized);
                    }
                    else
                    {
                        builder.Add(attr);
                    }
                }
            }
        }
    }
}
