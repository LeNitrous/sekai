// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of layout formats.
/// </summary>
public enum InputLayoutFormat
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

public static class InputLayoutFormatExtensions
{
    /// <summary>
    /// Gets the size in bytes of a given vertex member format.
    /// </summary>
    public static int SizeOfFormat(this InputLayoutFormat format)
    {
        switch (format)
        {
            case InputLayoutFormat.Byte:
            case InputLayoutFormat.UnsignedByte:
                return 1;
            case InputLayoutFormat.Short:
            case InputLayoutFormat.Half:
            case InputLayoutFormat.UnsignedShort:
                return 2;
            case InputLayoutFormat.Int:
            case InputLayoutFormat.Float:
            case InputLayoutFormat.UnsignedInt:
                return 4;
            case InputLayoutFormat.Double:
                return 8;
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }
}
