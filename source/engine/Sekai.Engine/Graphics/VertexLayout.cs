// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Engine.Graphics;

/// <summary>
/// Helper class for defining vertex layouts.
/// </summary>
public class VertexLayout : FrameworkObject
{
    public int Stride
    {
        get
        {
            if (dirty)
                stride = elements.Select(e => getElementCount(e.Format)).Sum();

            return stride;
        }
    }

    private int stride;
    private bool dirty;
    private readonly List<VertexElementDescription> elements = new();

    /// <summary>
    /// Appends a vertex element member of the given type to this layout.
    /// </summary>
    public VertexLayout Add<T>(string name)
        where T : unmanaged
    {
        return Add(typeof(T), name);
    }

    /// <summary>
    /// Appends a vertex element member of the given type to this layout.
    /// </summary>
    public VertexLayout Add(Type type, string name)
    {
        if (!formats.TryGetValue(type, out var format))
            throw new NotSupportedException($@"{type} is not a supported vertex format.");

        return Add(name, format);
    }

    /// <summary>
    /// Appends a vertex element member with a given format to this layout.
    /// </summary>
    public VertexLayout Add(string name, VertexElementFormat format)
    {
        var description = new VertexElementDescription
        {
            Name = name,
            Format = format
        };

        dirty = true;
        elements.Add(description);

        return this;
    }

    /// <summary>
    /// Appends another leyout to this layout.
    /// </summary>
    public VertexLayout Add(VertexLayout layout)
    {
        dirty = true;
        elements.AddRange(layout.elements);
        return this;
    }

    public VertexLayoutDescription Build()
    {
        return new VertexLayoutDescription((uint)Stride, elements.ToArray());
    }

    private static readonly Dictionary<Type, VertexElementFormat> formats = new()
    {
        { typeof(int), VertexElementFormat.Int1 },
        { typeof(uint), VertexElementFormat.UInt1 },
        { typeof(float), VertexElementFormat.Float1 },
        { typeof(Vector2), VertexElementFormat.Float2 },
        { typeof(Vector3), VertexElementFormat.Float3 },
        { typeof(Vector4), VertexElementFormat.Float4 },
    };

    private static int getElementCount(VertexElementFormat format)
    {

#pragma warning disable IDE0066 // Switch expression does not read well in this scenario.

        switch (format)
        {
            case VertexElementFormat.Byte2_Norm:
            case VertexElementFormat.Byte2:
            case VertexElementFormat.SByte2_Norm:
            case VertexElementFormat.SByte2:
            case VertexElementFormat.Half1:
                return 2;
            case VertexElementFormat.Float1:
            case VertexElementFormat.UInt1:
            case VertexElementFormat.Int1:
            case VertexElementFormat.Byte4_Norm:
            case VertexElementFormat.Byte4:
            case VertexElementFormat.SByte4_Norm:
            case VertexElementFormat.SByte4:
            case VertexElementFormat.UShort2_Norm:
            case VertexElementFormat.UShort2:
            case VertexElementFormat.Short2_Norm:
            case VertexElementFormat.Short2:
            case VertexElementFormat.Half2:
                return 4;
            case VertexElementFormat.Float2:
            case VertexElementFormat.UInt2:
            case VertexElementFormat.Int2:
            case VertexElementFormat.UShort4_Norm:
            case VertexElementFormat.UShort4:
            case VertexElementFormat.Short4_Norm:
            case VertexElementFormat.Short4:
            case VertexElementFormat.Half4:
                return 8;
            case VertexElementFormat.Float3:
            case VertexElementFormat.UInt3:
            case VertexElementFormat.Int3:
                return 12;
            case VertexElementFormat.Float4:
            case VertexElementFormat.UInt4:
            case VertexElementFormat.Int4:
                return 16;
            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }

#pragma warning restore IDE0066

    }
}
