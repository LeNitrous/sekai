// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Annotations;

[AttributeUsage(AttributeTargets.Field)]
public class VertexMemberAttribute : Attribute
{
    /// <summary>
    /// The vertex member name.
    /// </summary>
    public string? Name;

    /// <summary>
    /// The vertex member format.
    /// </summary>
    public VertexElementFormat? Format;

    public VertexMemberAttribute()
    {
    }

    public VertexMemberAttribute(string name)
    {
        Name = name;
    }

    public VertexMemberAttribute(VertexElementFormat format)
    {
        Format = format;
    }

    public VertexMemberAttribute(string name, VertexElementFormat format)
    {
        Name = name;
        Format = format;
    }
}
