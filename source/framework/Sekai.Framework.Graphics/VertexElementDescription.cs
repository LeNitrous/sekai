// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

public struct VertexElementDescription : IEquatable<VertexElementDescription>
{
    /// <summary>
    /// The name of the element.
    /// </summary>
    public string Name;

    /// <summary>
    /// The offset in bytes from the beginning of the vertex.
    /// </summary>
    public uint Offset;

    /// <summary>
    /// The format of this element.
    /// </summary>
    public VertexElementFormat Format;

    public VertexElementDescription(string name, uint offset, VertexElementFormat format)
    {
        Name = name;
        Offset = offset;
        Format = format;
    }

    public override bool Equals(object? obj)
    {
        return obj is VertexElementDescription description && Equals(description);
    }

    public bool Equals(VertexElementDescription other)
    {
        return Name == other.Name &&
               Offset == other.Offset &&
               Format == other.Format;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Offset, Format);
    }

    public static bool operator ==(VertexElementDescription left, VertexElementDescription right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(VertexElementDescription left, VertexElementDescription right)
    {
        return !(left == right);
    }
}
