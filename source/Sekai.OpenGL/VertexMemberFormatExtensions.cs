// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Vertices;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

public static class VertexMemberFormatExtensions
{
    public static VertexAttribPointerType ToVertexAttribPointerType(this VertexMemberFormat format)
    {
        switch (format)
        {
            case VertexMemberFormat.Byte:
                return VertexAttribPointerType.Byte;
            case VertexMemberFormat.UnsignedByte:
                return VertexAttribPointerType.UnsignedByte;
            case VertexMemberFormat.Short:
                return VertexAttribPointerType.Short;
            case VertexMemberFormat.UnsignedShort:
                return VertexAttribPointerType.UnsignedShort;
            case VertexMemberFormat.Int:
                return VertexAttribPointerType.Int;
            case VertexMemberFormat.UnsignedInt:
                return VertexAttribPointerType.UnsignedInt;
            case VertexMemberFormat.Half:
                return VertexAttribPointerType.HalfFloat;
            case VertexMemberFormat.Float:
                return VertexAttribPointerType.Float;
            case VertexMemberFormat.Double:
                return VertexAttribPointerType.Double;
            default:
                throw new NotSupportedException($@"Format ""{format}"" is not supported.");
        }
    }
}
