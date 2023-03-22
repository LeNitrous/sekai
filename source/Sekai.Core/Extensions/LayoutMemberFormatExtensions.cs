// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Veldrid;

namespace Sekai.Extensions;

internal static class LayoutMemberFormatExtensions
{
    /// <summary>
    /// Gets the size in bytes of a given layout member format.
    /// </summary>
    public static int SizeOfFormat(this LayoutMemberFormat format)
    {
        switch (format)
        {
            case LayoutMemberFormat.Byte:
            case LayoutMemberFormat.UnsignedByte:
                return 1;

            case LayoutMemberFormat.Short:
            case LayoutMemberFormat.Half:
            case LayoutMemberFormat.UnsignedShort:
                return 2;

            case LayoutMemberFormat.Int:
            case LayoutMemberFormat.Float:
            case LayoutMemberFormat.UnsignedInt:
                return 4;

            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    /// <summary>
    /// Gets the <see cref="VertexElementFormat"/> of a given <see cref="LayoutMemberFormat"/>.
    /// </summary>
    /// <param name="format">The format to retrieve.</param>
    /// <param name="count">The component count.</param>
    /// <param name="normalized">Whether the value should be normalized or not.</param>
    public static VertexElementFormat AsVertexFormat(this LayoutMemberFormat format, int count, bool normalized)
    {
        switch (format)
        {
            case LayoutMemberFormat.Byte when count == 2 && !normalized:
                return VertexElementFormat.SByte2;

            case LayoutMemberFormat.Byte when count == 2 && normalized:
                return VertexElementFormat.SByte2_Norm;

            case LayoutMemberFormat.Byte when count == 4 && !normalized:
                return VertexElementFormat.SByte4;

            case LayoutMemberFormat.Byte when count == 4 && normalized:
                return VertexElementFormat.SByte4_Norm;

            case LayoutMemberFormat.UnsignedByte when count == 2 && !normalized:
                return VertexElementFormat.Byte2;

            case LayoutMemberFormat.UnsignedByte when count == 2 && normalized:
                return VertexElementFormat.Byte2_Norm;

            case LayoutMemberFormat.UnsignedByte when count == 4 && !normalized:
                return VertexElementFormat.Byte4;

            case LayoutMemberFormat.UnsignedByte when count == 4 && normalized:
                return VertexElementFormat.Byte4_Norm;

            case LayoutMemberFormat.Short when count == 2 && !normalized:
                return VertexElementFormat.Short2;

            case LayoutMemberFormat.Short when count == 2 && normalized:
                return VertexElementFormat.Short2_Norm;

            case LayoutMemberFormat.Short when count == 4 && !normalized:
                return VertexElementFormat.Short4;

            case LayoutMemberFormat.Short when count == 4 && normalized:
                return VertexElementFormat.Short4_Norm;

            case LayoutMemberFormat.UnsignedShort when count == 2 && !normalized:
                return VertexElementFormat.UShort2;

            case LayoutMemberFormat.UnsignedShort when count == 2 && normalized:
                return VertexElementFormat.UShort2_Norm;

            case LayoutMemberFormat.UnsignedShort when count == 4 && !normalized:
                return VertexElementFormat.UShort4;

            case LayoutMemberFormat.UnsignedShort when count == 4 && normalized:
                return VertexElementFormat.UShort4_Norm;

            case LayoutMemberFormat.Int when count == 1:
                return VertexElementFormat.Int1;

            case LayoutMemberFormat.Int when count == 2:
                return VertexElementFormat.Int2;

            case LayoutMemberFormat.Int when count == 3:
                return VertexElementFormat.Int3;

            case LayoutMemberFormat.Int when count == 4:
                return VertexElementFormat.Int4;

            case LayoutMemberFormat.UnsignedInt when count == 1:
                return VertexElementFormat.UInt1;

            case LayoutMemberFormat.UnsignedInt when count == 2:
                return VertexElementFormat.UInt2;

            case LayoutMemberFormat.UnsignedInt when count == 3:
                return VertexElementFormat.UInt3;

            case LayoutMemberFormat.UnsignedInt when count == 4:
                return VertexElementFormat.UInt4;

            case LayoutMemberFormat.Half when count == 1:
                return VertexElementFormat.Half1;

            case LayoutMemberFormat.Half when count == 2:
                return VertexElementFormat.Half2;

            case LayoutMemberFormat.Half when count == 4:
                return VertexElementFormat.Half4;

            case LayoutMemberFormat.Float when count == 1:
                return VertexElementFormat.Float1;

            case LayoutMemberFormat.Float when count == 2:
                return VertexElementFormat.Float2;

            case LayoutMemberFormat.Float when count == 3:
                return VertexElementFormat.Float3;

            case LayoutMemberFormat.Float when count == 4:
                return VertexElementFormat.Float4;

            default:
                throw new NotSupportedException($"The layout format {format} with {count} {(normalized ? "normalized " : string.Empty)}components is not supported.");
        }
    }
}
