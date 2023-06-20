// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;
using Sekai.Framework.Graphics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal static class GLExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BlendingFactor AsFactor(this BlendType type) => type switch
    {
        BlendType.Zero => BlendingFactor.Zero,
        BlendType.One => BlendingFactor.One,
        BlendType.SourceColor => BlendingFactor.SrcColor,
        BlendType.OneMinusSourceColor => BlendingFactor.OneMinusSrcColor,
        BlendType.DestinationColor => BlendingFactor.DstColor,
        BlendType.OneMinusDestinationColor => BlendingFactor.OneMinusDstColor,
        BlendType.SourceAlpha => BlendingFactor.SrcAlpha,
        BlendType.OneMinusSourceAlpha => BlendingFactor.OneMinusSrcAlpha,
        BlendType.DestinationAlpha => BlendingFactor.DstAlpha,
        BlendType.OneMinusDestinationAlpha => BlendingFactor.OneMinusDstAlpha,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BlendEquationModeEXT AsEquation(this BlendOperation operation) => operation switch
    {
        BlendOperation.Add => BlendEquationModeEXT.FuncAdd,
        BlendOperation.Subtract => BlendEquationModeEXT.FuncSubtract,
        BlendOperation.ReverseSubtract => BlendEquationModeEXT.FuncReverseSubtract,
        BlendOperation.Minimum => BlendEquationModeEXT.Min,
        BlendOperation.Maximum => BlendEquationModeEXT.Max,
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static GLEnum AsEnum(this ComparisonKind kind) => kind switch
    {
        ComparisonKind.Never => GLEnum.Never,
        ComparisonKind.Less => GLEnum.Less,
        ComparisonKind.Equal => GLEnum.Equal,
        ComparisonKind.LessEqual => GLEnum.Lequal,
        ComparisonKind.Greater => GLEnum.Greater,
        ComparisonKind.NotEqual => GLEnum.Notequal,
        ComparisonKind.GreaterEqual => GLEnum.Gequal,
        ComparisonKind.Always => GLEnum.Always,
        _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static StencilOp AsOperation(this StencilOperation operation) => operation switch
    {
        StencilOperation.Keep => StencilOp.Keep,
        StencilOperation.Zero => StencilOp.Zero,
        StencilOperation.Replace => StencilOp.Replace,
        StencilOperation.Increment => StencilOp.Incr,
        StencilOperation.IncrementWrap => StencilOp.IncrWrap,
        StencilOperation.Decrement => StencilOp.Decr,
        StencilOperation.DecrementWrap => StencilOp.DecrWrap,
        StencilOperation.Invert => StencilOp.Invert,
        _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PolygonMode AsPolygonMode(this FillMode mode) => mode switch
    {
        FillMode.Solid => PolygonMode.Fill,
        FillMode.Wireframe => PolygonMode.Line,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FrontFaceDirection AsDirection(this FaceWinding wind) => wind switch
    {
        FaceWinding.Clockwise => FrontFaceDirection.CW,
        FaceWinding.CounterClockwise => FrontFaceDirection.Ccw,
        _ => throw new ArgumentOutOfRangeException(nameof(wind), wind, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TriangleFace AsTriangleFace(this FaceCulling cull) => cull switch
    {
        FaceCulling.Back => TriangleFace.Back,
        FaceCulling.Front => TriangleFace.Front,
        _ => throw new ArgumentOutOfRangeException(nameof(cull), cull, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Silk.NET.OpenGL.PrimitiveType AsPrimitiveType(this Framework.Graphics.PrimitiveType type) => type switch
    {
        Framework.Graphics.PrimitiveType.TriangleList => Silk.NET.OpenGL.PrimitiveType.Triangles,
        Framework.Graphics.PrimitiveType.TriangleStrip => Silk.NET.OpenGL.PrimitiveType.TriangleStrip,
        Framework.Graphics.PrimitiveType.LineList => Silk.NET.OpenGL.PrimitiveType.Lines,
        Framework.Graphics.PrimitiveType.LineStrip => Silk.NET.OpenGL.PrimitiveType.LineStrip,
        Framework.Graphics.PrimitiveType.PointList => Silk.NET.OpenGL.PrimitiveType.Points,
        Framework.Graphics.PrimitiveType.TriangleListAdjacency => Silk.NET.OpenGL.PrimitiveType.TrianglesAdjacency,
        Framework.Graphics.PrimitiveType.TriangleStripAdjacency => Silk.NET.OpenGL.PrimitiveType.TriangleStripAdjacency,
        Framework.Graphics.PrimitiveType.LineListAdjacency => Silk.NET.OpenGL.PrimitiveType.LinesAdjacency,
        Framework.Graphics.PrimitiveType.LineStripAdjacency => Silk.NET.OpenGL.PrimitiveType.LineStripAdjacency,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BufferTargetARB AsTarget(this BufferType type) => type switch
    {
        BufferType.Vertex => BufferTargetARB.ArrayBuffer,
        BufferType.Index => BufferTargetARB.ElementArrayBuffer,
        BufferType.Uniform => BufferTargetARB.UniformBuffer,
        BufferType.Storage => BufferTargetARB.ShaderStorageBuffer,
        BufferType.Indirect => BufferTargetARB.DrawIndirectBuffer,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static BufferAccessARB AsAccess(this MapMode mode) => mode switch
    {
        MapMode.Read => BufferAccessARB.ReadOnly,
        MapMode.Write => BufferAccessARB.WriteOnly,
        MapMode.ReadWrite => BufferAccessARB.ReadWrite,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VertexAttribPointerType AsAttribType(this VertexMemberFormat format) => format switch
    {
        VertexMemberFormat.Byte => VertexAttribPointerType.Byte,
        VertexMemberFormat.UnsignedByte => VertexAttribPointerType.UnsignedByte,
        VertexMemberFormat.Short => VertexAttribPointerType.Short,
        VertexMemberFormat.UnsignedShort => VertexAttribPointerType.UnsignedShort,
        VertexMemberFormat.Int => VertexAttribPointerType.Int,
        VertexMemberFormat.UnsignedInt => VertexAttribPointerType.UnsignedInt,
        VertexMemberFormat.Half => VertexAttribPointerType.HalfFloat,
        VertexMemberFormat.Float => VertexAttribPointerType.Float,
        _ => throw new ArgumentOutOfRangeException(nameof(format), format, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DrawElementsType AsElementsType(this IndexType type) => type switch
    {
        IndexType.Short => DrawElementsType.UnsignedShort,
        IndexType.UnsignedShort => DrawElementsType.UnsignedShort,
        IndexType.Int => DrawElementsType.UnsignedInt,
        IndexType.UnsignedInt => DrawElementsType.UnsignedInt,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AsFilters(this TextureFilter filter, out TextureMinFilter minFilter, out TextureMagFilter magFilter)
    {
        switch (filter)
        {
            case TextureFilter.Anisotropic:
                minFilter = TextureMinFilter.LinearMipmapLinear;
                magFilter = TextureMagFilter.Linear;
                break;

            case TextureFilter.MinMagMipPoint:
                minFilter = TextureMinFilter.NearestMipmapNearest;
                magFilter = TextureMagFilter.Nearest;
                break;

            case TextureFilter.MinMagPointMipLinear:
                minFilter = TextureMinFilter.NearestMipmapLinear;
                magFilter = TextureMagFilter.Nearest;
                break;

            case TextureFilter.MinPointMagLinearMipPoint:
                minFilter = TextureMinFilter.NearestMipmapNearest;
                magFilter = TextureMagFilter.Linear;
                break;

            case TextureFilter.MinPointMagMipLinear:
                minFilter = TextureMinFilter.NearestMipmapLinear;
                magFilter = TextureMagFilter.Linear;
                break;

            case TextureFilter.MinLinearMagMipPoint:
                minFilter = TextureMinFilter.LinearMipmapNearest;
                magFilter = TextureMagFilter.Nearest;
                break;

            case TextureFilter.MinLinearMagPointMipLinear:
                minFilter = TextureMinFilter.LinearMipmapLinear;
                magFilter = TextureMagFilter.Nearest;
                break;

            case TextureFilter.MinMagLinearMipPoint:
                minFilter = TextureMinFilter.LinearMipmapNearest;
                magFilter = TextureMagFilter.Linear;
                break;

            case TextureFilter.MinMagMipLinear:
                minFilter = TextureMinFilter.LinearMipmapLinear;
                magFilter = TextureMagFilter.Linear;
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureWrapMode AsWrapMode(this TextureAddress address) => address switch
    {
        TextureAddress.Repeat => TextureWrapMode.Repeat,
        TextureAddress.Mirror => TextureWrapMode.MirroredRepeat,
        TextureAddress.ClampToEdge => TextureWrapMode.ClampToEdge,
        TextureAddress.ClampToBorder => TextureWrapMode.ClampToBorder,
        _ => throw new ArgumentOutOfRangeException(nameof(address), address, null),
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PixelType AsPixelType(this Framework.Graphics.PixelFormat format)
    {
        switch (format)
        {
            case Framework.Graphics.PixelFormat.R8_UNorm:
            case Framework.Graphics.PixelFormat.R8_UInt:
            case Framework.Graphics.PixelFormat.R8G8_UNorm:
            case Framework.Graphics.PixelFormat.R8G8_UInt:
            case Framework.Graphics.PixelFormat.R8G8B8A8_UNorm:
            case Framework.Graphics.PixelFormat.R8G8B8A8_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.R8G8B8A8_UInt:
            case Framework.Graphics.PixelFormat.B8G8R8A8_UNorm:
            case Framework.Graphics.PixelFormat.B8G8R8A8_UNorm_SRgb:
                return PixelType.UnsignedByte;
            case Framework.Graphics.PixelFormat.R8_SNorm:
            case Framework.Graphics.PixelFormat.R8_SInt:
            case Framework.Graphics.PixelFormat.R8G8_SNorm:
            case Framework.Graphics.PixelFormat.R8G8_SInt:
            case Framework.Graphics.PixelFormat.R8G8B8A8_SNorm:
            case Framework.Graphics.PixelFormat.R8G8B8A8_SInt:
            case Framework.Graphics.PixelFormat.BC4_SNorm:
            case Framework.Graphics.PixelFormat.BC5_SNorm:
                return PixelType.Byte;
            case Framework.Graphics.PixelFormat.R16_UNorm:
            case Framework.Graphics.PixelFormat.R16_UInt:
            case Framework.Graphics.PixelFormat.R16G16_UNorm:
            case Framework.Graphics.PixelFormat.R16G16_UInt:
            case Framework.Graphics.PixelFormat.R16G16B16A16_UNorm:
            case Framework.Graphics.PixelFormat.R16G16B16A16_UInt:
                return PixelType.UnsignedShort;
            case Framework.Graphics.PixelFormat.R16_SNorm:
            case Framework.Graphics.PixelFormat.R16_SInt:
            case Framework.Graphics.PixelFormat.R16G16_SNorm:
            case Framework.Graphics.PixelFormat.R16G16_SInt:
            case Framework.Graphics.PixelFormat.R16G16B16A16_SNorm:
            case Framework.Graphics.PixelFormat.R16G16B16A16_SInt:
                return PixelType.Short;
            case Framework.Graphics.PixelFormat.R32_UInt:
            case Framework.Graphics.PixelFormat.R32G32_UInt:
            case Framework.Graphics.PixelFormat.R32G32B32A32_UInt:
                return PixelType.UnsignedInt;
            case Framework.Graphics.PixelFormat.R32_SInt:
            case Framework.Graphics.PixelFormat.R32G32_SInt:
            case Framework.Graphics.PixelFormat.R32G32B32A32_SInt:
                return PixelType.Int;
            case Framework.Graphics.PixelFormat.R16_Float:
            case Framework.Graphics.PixelFormat.R16G16_Float:
            case Framework.Graphics.PixelFormat.R16G16B16A16_Float:
                return (PixelType)5131;
            case Framework.Graphics.PixelFormat.R32_Float:
            case Framework.Graphics.PixelFormat.R32G32_Float:
            case Framework.Graphics.PixelFormat.R32G32B32A32_Float:
                return PixelType.Float;

            case Framework.Graphics.PixelFormat.BC1_RGB_UNorm:
            case Framework.Graphics.PixelFormat.BC1_RGB_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC1_RGBA_UNorm:
            case Framework.Graphics.PixelFormat.BC1_RGBA_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC2_UNorm:
            case Framework.Graphics.PixelFormat.BC2_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC3_UNorm:
            case Framework.Graphics.PixelFormat.BC3_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC4_UNorm:
            case Framework.Graphics.PixelFormat.BC5_UNorm:
            case Framework.Graphics.PixelFormat.BC7_UNorm:
            case Framework.Graphics.PixelFormat.BC7_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8_UNorm:
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8A1_UNorm:
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8A8_UNorm:
                return PixelType.UnsignedByte;

            case Framework.Graphics.PixelFormat.D32_Float_S8_UInt:
                return (PixelType)36269;

            case Framework.Graphics.PixelFormat.D24_UNorm_S8_UInt:
                return (PixelType)34042;

            case Framework.Graphics.PixelFormat.R10G10B10A2_UNorm:
            case Framework.Graphics.PixelFormat.R10G10B10A2_UInt:
                return PixelType.UnsignedInt1010102;

            case Framework.Graphics.PixelFormat.R11G11B10_Float:
                return (PixelType)35889;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Silk.NET.OpenGL.PixelFormat AsPixelFormat(this Framework.Graphics.PixelFormat format)
    {
        switch (format)
        {
            case Framework.Graphics.PixelFormat.R8_UNorm:
            case Framework.Graphics.PixelFormat.R16_UNorm:
            case Framework.Graphics.PixelFormat.R16_Float:
            case Framework.Graphics.PixelFormat.R32_Float:
            case Framework.Graphics.PixelFormat.BC4_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Red;

            case Framework.Graphics.PixelFormat.R8_SNorm:
            case Framework.Graphics.PixelFormat.R8_UInt:
            case Framework.Graphics.PixelFormat.R8_SInt:
            case Framework.Graphics.PixelFormat.R16_SNorm:
            case Framework.Graphics.PixelFormat.R16_UInt:
            case Framework.Graphics.PixelFormat.R16_SInt:
            case Framework.Graphics.PixelFormat.R32_UInt:
            case Framework.Graphics.PixelFormat.R32_SInt:
            case Framework.Graphics.PixelFormat.BC4_SNorm:
                return Silk.NET.OpenGL.PixelFormat.RedInteger;

            case Framework.Graphics.PixelFormat.R8G8_UNorm:
            case Framework.Graphics.PixelFormat.R16G16_UNorm:
            case Framework.Graphics.PixelFormat.R16G16_Float:
            case Framework.Graphics.PixelFormat.R32G32_Float:
            case Framework.Graphics.PixelFormat.BC5_UNorm:
                return Silk.NET.OpenGL.PixelFormat.RG;

            case Framework.Graphics.PixelFormat.R8G8_SNorm:
            case Framework.Graphics.PixelFormat.R8G8_UInt:
            case Framework.Graphics.PixelFormat.R8G8_SInt:
            case Framework.Graphics.PixelFormat.R16G16_SNorm:
            case Framework.Graphics.PixelFormat.R16G16_UInt:
            case Framework.Graphics.PixelFormat.R16G16_SInt:
            case Framework.Graphics.PixelFormat.R32G32_UInt:
            case Framework.Graphics.PixelFormat.R32G32_SInt:
            case Framework.Graphics.PixelFormat.BC5_SNorm:
                return Silk.NET.OpenGL.PixelFormat.RGInteger;

            case Framework.Graphics.PixelFormat.R8G8B8A8_UNorm:
            case Framework.Graphics.PixelFormat.R8G8B8A8_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.R16G16B16A16_UNorm:
            case Framework.Graphics.PixelFormat.R16G16B16A16_Float:
            case Framework.Graphics.PixelFormat.R32G32B32A32_Float:
                return Silk.NET.OpenGL.PixelFormat.Rgba;

            case Framework.Graphics.PixelFormat.B8G8R8A8_UNorm:
            case Framework.Graphics.PixelFormat.B8G8R8A8_UNorm_SRgb:
                return Silk.NET.OpenGL.PixelFormat.Bgra;

            case Framework.Graphics.PixelFormat.R8G8B8A8_SNorm:
            case Framework.Graphics.PixelFormat.R8G8B8A8_UInt:
            case Framework.Graphics.PixelFormat.R8G8B8A8_SInt:
            case Framework.Graphics.PixelFormat.R16G16B16A16_SNorm:
            case Framework.Graphics.PixelFormat.R16G16B16A16_UInt:
            case Framework.Graphics.PixelFormat.R16G16B16A16_SInt:
            case Framework.Graphics.PixelFormat.R32G32B32A32_UInt:
            case Framework.Graphics.PixelFormat.R32G32B32A32_SInt:
                return Silk.NET.OpenGL.PixelFormat.RgbaInteger;

            case Framework.Graphics.PixelFormat.BC1_RGB_UNorm:
            case Framework.Graphics.PixelFormat.BC1_RGB_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Rgb;
            case Framework.Graphics.PixelFormat.BC1_RGBA_UNorm:
            case Framework.Graphics.PixelFormat.BC1_RGBA_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC2_UNorm:
            case Framework.Graphics.PixelFormat.BC2_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC3_UNorm:
            case Framework.Graphics.PixelFormat.BC3_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.BC7_UNorm:
            case Framework.Graphics.PixelFormat.BC7_UNorm_SRgb:
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8A1_UNorm:
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8A8_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Rgba;

            case Framework.Graphics.PixelFormat.D24_UNorm_S8_UInt:
            case Framework.Graphics.PixelFormat.D32_Float_S8_UInt:
                return Silk.NET.OpenGL.PixelFormat.DepthStencil;

            case Framework.Graphics.PixelFormat.R10G10B10A2_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Rgba;
            case Framework.Graphics.PixelFormat.R10G10B10A2_UInt:
                return Silk.NET.OpenGL.PixelFormat.RgbaInteger;
            case Framework.Graphics.PixelFormat.R11G11B10_Float:
                return Silk.NET.OpenGL.PixelFormat.Rgb;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static InternalFormat AsInternalFormat(this Framework.Graphics.PixelFormat format)
    {
        switch (format)
        {
            case Framework.Graphics.PixelFormat.R8_UNorm:
                return InternalFormat.R8;
            case Framework.Graphics.PixelFormat.R8_SNorm:
                return InternalFormat.R8SNorm;
            case Framework.Graphics.PixelFormat.R8_UInt:
                return InternalFormat.R8ui;
            case Framework.Graphics.PixelFormat.R8_SInt:
                return InternalFormat.R8i;

            case Framework.Graphics.PixelFormat.R16_UNorm:
                return InternalFormat.R16;
            case Framework.Graphics.PixelFormat.R16_SNorm:
                return InternalFormat.R16SNorm;
            case Framework.Graphics.PixelFormat.R16_UInt:
                return InternalFormat.R16ui;
            case Framework.Graphics.PixelFormat.R16_SInt:
                return InternalFormat.R16i;
            case Framework.Graphics.PixelFormat.R16_Float:
                return InternalFormat.R16f;
            case Framework.Graphics.PixelFormat.R32_UInt:
                return InternalFormat.R32ui;
            case Framework.Graphics.PixelFormat.R32_SInt:
                return InternalFormat.R32i;
            case Framework.Graphics.PixelFormat.R32_Float:
                return InternalFormat.R32f;

            case Framework.Graphics.PixelFormat.R8G8_UNorm:
                return InternalFormat.RG8;
            case Framework.Graphics.PixelFormat.R8G8_SNorm:
                return InternalFormat.RG8SNorm;
            case Framework.Graphics.PixelFormat.R8G8_UInt:
                return InternalFormat.RG8ui;
            case Framework.Graphics.PixelFormat.R8G8_SInt:
                return InternalFormat.RG8i;

            case Framework.Graphics.PixelFormat.R16G16_UNorm:
                return InternalFormat.RG16;
            case Framework.Graphics.PixelFormat.R16G16_SNorm:
                return InternalFormat.RG16SNorm;
            case Framework.Graphics.PixelFormat.R16G16_UInt:
                return InternalFormat.RG16ui;
            case Framework.Graphics.PixelFormat.R16G16_SInt:
                return InternalFormat.RG16i;
            case Framework.Graphics.PixelFormat.R16G16_Float:
                return InternalFormat.RG16f;

            case Framework.Graphics.PixelFormat.R32G32_UInt:
                return InternalFormat.RG32ui;
            case Framework.Graphics.PixelFormat.R32G32_SInt:
                return InternalFormat.RG32i;
            case Framework.Graphics.PixelFormat.R32G32_Float:
                return InternalFormat.RG32f;

            case Framework.Graphics.PixelFormat.R8G8B8A8_UNorm:
                return InternalFormat.Rgba8;
            case Framework.Graphics.PixelFormat.R8G8B8A8_UNorm_SRgb:
                return InternalFormat.Srgb8Alpha8;
            case Framework.Graphics.PixelFormat.R8G8B8A8_SNorm:
                return InternalFormat.Rgba8SNorm;
            case Framework.Graphics.PixelFormat.R8G8B8A8_UInt:
                return InternalFormat.Rgba8ui;
            case Framework.Graphics.PixelFormat.R8G8B8A8_SInt:
                return InternalFormat.Rgba8i;

            case Framework.Graphics.PixelFormat.R16G16B16A16_UNorm:
                return InternalFormat.Rgba16;
            case Framework.Graphics.PixelFormat.R16G16B16A16_SNorm:
                return InternalFormat.Rgba16SNorm;
            case Framework.Graphics.PixelFormat.R16G16B16A16_UInt:
                return InternalFormat.Rgba16ui;
            case Framework.Graphics.PixelFormat.R16G16B16A16_SInt:
                return InternalFormat.Rgba16i;
            case Framework.Graphics.PixelFormat.R16G16B16A16_Float:
                return InternalFormat.Rgba16f;

            case Framework.Graphics.PixelFormat.R32G32B32A32_Float:
                return InternalFormat.Rgba32f;
            case Framework.Graphics.PixelFormat.R32G32B32A32_UInt:
                return InternalFormat.Rgba32ui;
            case Framework.Graphics.PixelFormat.R32G32B32A32_SInt:
                return InternalFormat.Rgba32i;

            case Framework.Graphics.PixelFormat.B8G8R8A8_UNorm:
                return InternalFormat.Rgba;
            case Framework.Graphics.PixelFormat.B8G8R8A8_UNorm_SRgb:
                return InternalFormat.Srgb8Alpha8;

            case Framework.Graphics.PixelFormat.BC1_RGB_UNorm:
                return InternalFormat.CompressedRgbS3TCDxt1Ext;
            case Framework.Graphics.PixelFormat.BC1_RGB_UNorm_SRgb:
                return InternalFormat.CompressedSrgbS3TCDxt1Ext;
            case Framework.Graphics.PixelFormat.BC1_RGBA_UNorm:
                return InternalFormat.CompressedRgbaS3TCDxt1Ext;
            case Framework.Graphics.PixelFormat.BC1_RGBA_UNorm_SRgb:
                return InternalFormat.CompressedSrgbAlphaS3TCDxt1Ext;
            case Framework.Graphics.PixelFormat.BC2_UNorm:
                return InternalFormat.CompressedRgbaS3TCDxt3Ext;
            case Framework.Graphics.PixelFormat.BC2_UNorm_SRgb:
                return InternalFormat.CompressedSrgbAlphaS3TCDxt3Ext;
            case Framework.Graphics.PixelFormat.BC3_UNorm:
                return InternalFormat.CompressedRgbaS3TCDxt5Ext;
            case Framework.Graphics.PixelFormat.BC3_UNorm_SRgb:
                return InternalFormat.CompressedSrgbAlphaS3TCDxt5Ext;
            case Framework.Graphics.PixelFormat.BC4_UNorm:
                return InternalFormat.CompressedRedRgtc1;
            case Framework.Graphics.PixelFormat.BC4_SNorm:
                return InternalFormat.CompressedSignedRedRgtc1;
            case Framework.Graphics.PixelFormat.BC5_UNorm:
                return InternalFormat.CompressedRGRgtc2;
            case Framework.Graphics.PixelFormat.BC5_SNorm:
                return InternalFormat.CompressedSignedRGRgtc2;
            case Framework.Graphics.PixelFormat.BC7_UNorm:
                return InternalFormat.CompressedRgbaBptcUnorm;
            case Framework.Graphics.PixelFormat.BC7_UNorm_SRgb:
                return InternalFormat.CompressedSrgbAlphaBptcUnorm;

            case Framework.Graphics.PixelFormat.ETC2_R8G8B8_UNorm:
                return InternalFormat.CompressedRgb8Etc2;
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8A1_UNorm:
                return InternalFormat.CompressedRgb8PunchthroughAlpha1Etc2;
            case Framework.Graphics.PixelFormat.ETC2_R8G8B8A8_UNorm:
                return InternalFormat.CompressedRgba8Etc2Eac;

            case Framework.Graphics.PixelFormat.D32_Float_S8_UInt:
                return InternalFormat.Depth32fStencil8;
            case Framework.Graphics.PixelFormat.D24_UNorm_S8_UInt:
                return InternalFormat.Depth24Stencil8;

            case Framework.Graphics.PixelFormat.R10G10B10A2_UNorm:
                return InternalFormat.Rgb10A2;
            case Framework.Graphics.PixelFormat.R10G10B10A2_UInt:
                return InternalFormat.Rgb10A2ui;
            case Framework.Graphics.PixelFormat.R11G11B10_Float:
                return InternalFormat.R11fG11fB10f;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TextureTarget AsTarget(this TextureDescription description)
    {
        switch (description.Type)
        {
            case TextureType.Cubemap when description.Layers > 1:
                return TextureTarget.TextureCubeMapArray;

            case TextureType.Cubemap when description.Layers == 1:
                return TextureTarget.TextureCubeMap;

            case TextureType.Texture1D when description.Layers > 1:
                return TextureTarget.Texture1DArray;

            case TextureType.Texture1D when description.Layers == 1:
                return TextureTarget.Texture1D;

            case TextureType.Texture2D when description.Layers > 1 && description.Count > TextureSampleCount.Count1:
                return TextureTarget.Texture2DMultisampleArray;

            case TextureType.Texture2D when description.Layers == 1 && description.Count > TextureSampleCount.Count1:
                return TextureTarget.Texture2DMultisample;

            case TextureType.Texture2D when description.Layers > 1 && description.Count == TextureSampleCount.Count1:
                return TextureTarget.Texture2DArray;

            case TextureType.Texture2D when description.Layers == 1 && description.Count == TextureSampleCount.Count1:
                return TextureTarget.Texture2D;

            case TextureType.Texture3D:
                return TextureTarget.Texture3D;

            default:
                throw new ArgumentOutOfRangeException(nameof(description), description, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ShaderType AsType(this ShaderStage stage) => stage switch
    {
        ShaderStage.Vertex => ShaderType.VertexShader,
        ShaderStage.Geometry => ShaderType.GeometryShader,
        ShaderStage.TesselationControl => ShaderType.TessControlShader,
        ShaderStage.TesselationEvaluation => ShaderType.TessEvaluationShader,
        ShaderStage.Fragment => ShaderType.FragmentShader,
        ShaderStage.Compute => ShaderType.ComputeShader,
        _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null),
    };
}
