// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Vertices;

[AttributeUsage(AttributeTargets.Field)]
public class VertexMemberAttribute : Attribute
{
    public readonly string? Name;
    public readonly VertexMemberFormat Format;

    public VertexMemberAttribute(string name, VertexMemberFormat format)
    {
        Name = name;
        Format = format;
    }

    public VertexMemberAttribute(VertexMemberFormat format)
    {
        Format = format;
    }
}
