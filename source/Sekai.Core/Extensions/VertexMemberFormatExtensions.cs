// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Veldrid;

namespace Sekai.Extensions;

internal static class VertexMemberFormatExtensions
{
    /// <summary>
    /// Gets the size in bytes of a given layout member format.
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

            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    /// <summary>
    /// Gets the <see cref="VertexElementFormat"/> of a given <see cref="VertexMemberFormat"/>.
    /// </summary>
    /// <param name="format">The format to retrieve.</param>
    /// <param name="count">The component count.</param>
    /// <param name="normalized">Whether the value should be normalized or not.</param>
    public static VertexElementFormat AsVertexFormat(this VertexMemberFormat format, int count, bool normalized)
    {
        switch (format)
        {
            case VertexMemberFormat.Byte when count == 2 && !normalized:
                return VertexElementFormat.SByte2;

            case VertexMemberFormat.Byte when count == 2 && normalized:
                return VertexElementFormat.SByte2_Norm;

            case VertexMemberFormat.Byte when count == 4 && !normalized:
                return VertexElementFormat.SByte4;

            case VertexMemberFormat.Byte when count == 4 && normalized:
                return VertexElementFormat.SByte4_Norm;

            case VertexMemberFormat.UnsignedByte when count == 2 && !normalized:
                return VertexElementFormat.Byte2;

            case VertexMemberFormat.UnsignedByte when count == 2 && normalized:
                return VertexElementFormat.Byte2_Norm;

            case VertexMemberFormat.UnsignedByte when count == 4 && !normalized:
                return VertexElementFormat.Byte4;

            case VertexMemberFormat.UnsignedByte when count == 4 && normalized:
                return VertexElementFormat.Byte4_Norm;

            case VertexMemberFormat.Short when count == 2 && !normalized:
                return VertexElementFormat.Short2;

            case VertexMemberFormat.Short when count == 2 && normalized:
                return VertexElementFormat.Short2_Norm;

            case VertexMemberFormat.Short when count == 4 && !normalized:
                return VertexElementFormat.Short4;

            case VertexMemberFormat.Short when count == 4 && normalized:
                return VertexElementFormat.Short4_Norm;

            case VertexMemberFormat.UnsignedShort when count == 2 && !normalized:
                return VertexElementFormat.UShort2;

            case VertexMemberFormat.UnsignedShort when count == 2 && normalized:
                return VertexElementFormat.UShort2_Norm;

            case VertexMemberFormat.UnsignedShort when count == 4 && !normalized:
                return VertexElementFormat.UShort4;

            case VertexMemberFormat.UnsignedShort when count == 4 && normalized:
                return VertexElementFormat.UShort4_Norm;

            case VertexMemberFormat.Int when count == 1:
                return VertexElementFormat.Int1;

            case VertexMemberFormat.Int when count == 2:
                return VertexElementFormat.Int2;

            case VertexMemberFormat.Int when count == 3:
                return VertexElementFormat.Int3;

            case VertexMemberFormat.Int when count == 4:
                return VertexElementFormat.Int4;

            case VertexMemberFormat.UnsignedInt when count == 1:
                return VertexElementFormat.UInt1;

            case VertexMemberFormat.UnsignedInt when count == 2:
                return VertexElementFormat.UInt2;

            case VertexMemberFormat.UnsignedInt when count == 3:
                return VertexElementFormat.UInt3;

            case VertexMemberFormat.UnsignedInt when count == 4:
                return VertexElementFormat.UInt4;

            case VertexMemberFormat.Half when count == 1:
                return VertexElementFormat.Half1;

            case VertexMemberFormat.Half when count == 2:
                return VertexElementFormat.Half2;

            case VertexMemberFormat.Half when count == 4:
                return VertexElementFormat.Half4;

            case VertexMemberFormat.Float when count == 1:
                return VertexElementFormat.Float1;

            case VertexMemberFormat.Float when count == 2:
                return VertexElementFormat.Float2;

            case VertexMemberFormat.Float when count == 3:
                return VertexElementFormat.Float3;

            case VertexMemberFormat.Float when count == 4:
                return VertexElementFormat.Float4;

            default:
                throw new NotSupportedException($"The layout format {format} with {count} {(normalized ? "normalized " : string.Empty)}components is not supported.");
        }
    }
}
