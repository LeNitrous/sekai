// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of input layout member formats.
/// </summary>
public enum VertexMemberFormat
{
    /// <summary>
    /// Byte.
    /// </summary>
    Byte,

    /// <summary>
    /// Unsigned byte.
    /// </summary>
    UnsignedByte,

    /// <summary>
    /// Short.
    /// </summary>
    Short,

    /// <summary>
    /// Unsigned short.
    /// </summary>
    UnsignedShort,

    /// <summary>
    /// Int.
    /// </summary>
    Int,

    /// <summary>
    /// Unisgned int.
    /// </summary>
    UnsignedInt,

    /// <summary>
    /// Half.
    /// </summary>
    Half,

    /// <summary>
    /// Float.
    /// </summary>
    Float,

    /// <summary>
    /// Double.
    /// </summary>
    Double,
}

public static class VertexMemberFormatExtensions
{
    /// <summary>
    /// Gets the size in bytes of a given vertex member format.
    /// </summary>
    public static int SizeOfFormat(this VertexMemberFormat format)
    {
        switch (format)
        {
            case VertexMemberFormat.Byte:
            case VertexMemberFormat.UnsignedByte:
                return 1;
            case VertexMemberFormat.Short:
            case VertexMemberFormat.Half:
            case VertexMemberFormat.UnsignedShort:
                return 2;
            case VertexMemberFormat.Int:
            case VertexMemberFormat.Float:
            case VertexMemberFormat.UnsignedInt:
                return 4;
            case VertexMemberFormat.Double:
                return 8;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}
