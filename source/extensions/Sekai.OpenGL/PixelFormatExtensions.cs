// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics.Textures;

namespace Sekai.OpenGL;

internal static class PixelFormatExtensions
{
    public static Silk.NET.OpenGL.PixelType ToPixelType(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.R8_UNorm:
            case PixelFormat.R8_UInt:
            case PixelFormat.R8_G8_UNorm:
            case PixelFormat.R8_G8_UInt:
            case PixelFormat.R8_G8_B8_A8_UNorm:
            case PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
            case PixelFormat.R8_G8_B8_A8_UInt:
            case PixelFormat.B8_G8_R8_A8_UNorm:
            case PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                return Silk.NET.OpenGL.PixelType.UnsignedByte;
            case PixelFormat.R8_SNorm:
            case PixelFormat.R8_SInt:
            case PixelFormat.R8_G8_SNorm:
            case PixelFormat.R8_G8_SInt:
            case PixelFormat.R8_G8_B8_A8_SNorm:
            case PixelFormat.R8_G8_B8_A8_SInt:
            case PixelFormat.BC4_SNorm:
            case PixelFormat.BC5_SNorm:
                return Silk.NET.OpenGL.PixelType.Byte;
            case PixelFormat.R16_UNorm:
            case PixelFormat.R16_UInt:
            case PixelFormat.R16_G16_UNorm:
            case PixelFormat.R16_G16_UInt:
            case PixelFormat.R16_G16_B16_A16_UNorm:
            case PixelFormat.R16_G16_B16_A16_UInt:
                return Silk.NET.OpenGL.PixelType.UnsignedShort;
            case PixelFormat.R16_SNorm:
            case PixelFormat.R16_SInt:
            case PixelFormat.R16_G16_SNorm:
            case PixelFormat.R16_G16_SInt:
            case PixelFormat.R16_G16_B16_A16_SNorm:
            case PixelFormat.R16_G16_B16_A16_SInt:
                return Silk.NET.OpenGL.PixelType.Short;
            case PixelFormat.R32_UInt:
            case PixelFormat.R32_G32_UInt:
            case PixelFormat.R32_G32_B32_A32_UInt:
                return Silk.NET.OpenGL.PixelType.UnsignedInt;
            case PixelFormat.R32_SInt:
            case PixelFormat.R32_G32_SInt:
            case PixelFormat.R32_G32_B32_A32_SInt:
                return Silk.NET.OpenGL.PixelType.Int;
            case PixelFormat.R16_Float:
            case PixelFormat.R16_G16_Float:
            case PixelFormat.R16_G16_B16_A16_Float:
                return (Silk.NET.OpenGL.PixelType)5131;
            case PixelFormat.R32_Float:
            case PixelFormat.R32_G32_Float:
            case PixelFormat.R32_G32_B32_A32_Float:
                return Silk.NET.OpenGL.PixelType.Float;

            case PixelFormat.BC1_Rgb_UNorm:
            case PixelFormat.BC1_Rgb_UNorm_SRgb:
            case PixelFormat.BC1_Rgba_UNorm:
            case PixelFormat.BC1_Rgba_UNorm_SRgb:
            case PixelFormat.BC2_UNorm:
            case PixelFormat.BC2_UNorm_SRgb:
            case PixelFormat.BC3_UNorm:
            case PixelFormat.BC3_UNorm_SRgb:
            case PixelFormat.BC4_UNorm:
            case PixelFormat.BC5_UNorm:
            case PixelFormat.BC7_UNorm:
            case PixelFormat.BC7_UNorm_SRgb:
            case PixelFormat.ETC2_R8_G8_B8_UNorm:
            case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
            case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                return Silk.NET.OpenGL.PixelType.UnsignedByte;

            case PixelFormat.D32_Float_S8_UInt:
                return (Silk.NET.OpenGL.PixelType)36269;
            case PixelFormat.D24_UNorm_S8_UInt:
                return (Silk.NET.OpenGL.PixelType)34042;

            case PixelFormat.R10_G10_B10_A2_UNorm:
            case PixelFormat.R10_G10_B10_A2_UInt:
                return Silk.NET.OpenGL.PixelType.UnsignedInt1010102;
            case PixelFormat.R11_G11_B10_Float:
                return (Silk.NET.OpenGL.PixelType)35889;

            default:
                throw new NotSupportedException(@$"Pixel format ""{format}"" is not supported.");
        }
    }

    public static Silk.NET.OpenGL.PixelFormat ToPixelFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.R8_UNorm:
            case PixelFormat.R16_UNorm:
            case PixelFormat.R16_Float:
            case PixelFormat.R32_Float:
            case PixelFormat.BC4_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Red;

            case PixelFormat.R8_SNorm:
            case PixelFormat.R8_UInt:
            case PixelFormat.R8_SInt:
            case PixelFormat.R16_SNorm:
            case PixelFormat.R16_UInt:
            case PixelFormat.R16_SInt:
            case PixelFormat.R32_UInt:
            case PixelFormat.R32_SInt:
            case PixelFormat.BC4_SNorm:
                return Silk.NET.OpenGL.PixelFormat.RedInteger;

            case PixelFormat.R8_G8_UNorm:
            case PixelFormat.R16_G16_UNorm:
            case PixelFormat.R16_G16_Float:
            case PixelFormat.R32_G32_Float:
            case PixelFormat.BC5_UNorm:
                return Silk.NET.OpenGL.PixelFormat.RG;

            case PixelFormat.R8_G8_SNorm:
            case PixelFormat.R8_G8_UInt:
            case PixelFormat.R8_G8_SInt:
            case PixelFormat.R16_G16_SNorm:
            case PixelFormat.R16_G16_UInt:
            case PixelFormat.R16_G16_SInt:
            case PixelFormat.R32_G32_UInt:
            case PixelFormat.R32_G32_SInt:
            case PixelFormat.BC5_SNorm:
                return Silk.NET.OpenGL.PixelFormat.RGInteger;

            case PixelFormat.R8_G8_B8_A8_UNorm:
            case PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
            case PixelFormat.R16_G16_B16_A16_UNorm:
            case PixelFormat.R16_G16_B16_A16_Float:
            case PixelFormat.R32_G32_B32_A32_Float:
                return Silk.NET.OpenGL.PixelFormat.Rgba;

            case PixelFormat.B8_G8_R8_A8_UNorm:
            case PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                return Silk.NET.OpenGL.PixelFormat.Bgra;

            case PixelFormat.R8_G8_B8_A8_SNorm:
            case PixelFormat.R8_G8_B8_A8_UInt:
            case PixelFormat.R8_G8_B8_A8_SInt:
            case PixelFormat.R16_G16_B16_A16_SNorm:
            case PixelFormat.R16_G16_B16_A16_UInt:
            case PixelFormat.R16_G16_B16_A16_SInt:
            case PixelFormat.R32_G32_B32_A32_UInt:
            case PixelFormat.R32_G32_B32_A32_SInt:
                return Silk.NET.OpenGL.PixelFormat.RgbaInteger;

            case PixelFormat.BC1_Rgb_UNorm:
            case PixelFormat.BC1_Rgb_UNorm_SRgb:
            case PixelFormat.ETC2_R8_G8_B8_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Rgb;
            case PixelFormat.BC1_Rgba_UNorm:
            case PixelFormat.BC1_Rgba_UNorm_SRgb:
            case PixelFormat.BC2_UNorm:
            case PixelFormat.BC2_UNorm_SRgb:
            case PixelFormat.BC3_UNorm:
            case PixelFormat.BC3_UNorm_SRgb:
            case PixelFormat.BC7_UNorm:
            case PixelFormat.BC7_UNorm_SRgb:
            case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
            case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Rgba;

            case PixelFormat.D24_UNorm_S8_UInt:
                return Silk.NET.OpenGL.PixelFormat.DepthStencil;
            case PixelFormat.D32_Float_S8_UInt:
                return Silk.NET.OpenGL.PixelFormat.DepthStencil;

            case PixelFormat.R10_G10_B10_A2_UNorm:
                return Silk.NET.OpenGL.PixelFormat.Rgba;
            case PixelFormat.R10_G10_B10_A2_UInt:
                return Silk.NET.OpenGL.PixelFormat.RgbaInteger;
            case PixelFormat.R11_G11_B10_Float:
                return Silk.NET.OpenGL.PixelFormat.Rgb;

            default:
                throw new NotSupportedException(@$"Pixel format ""{format}"" is not supported.");
        }
    }

    public static Silk.NET.OpenGL.InternalFormat ToInternalFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.R8_UNorm:
                return Silk.NET.OpenGL.InternalFormat.R8;
            case PixelFormat.R8_SNorm:
                return Silk.NET.OpenGL.InternalFormat.R8SNorm;
            case PixelFormat.R8_UInt:
                return Silk.NET.OpenGL.InternalFormat.R8ui;
            case PixelFormat.R8_SInt:
                return Silk.NET.OpenGL.InternalFormat.R8i;

            case PixelFormat.R16_UNorm:
                return Silk.NET.OpenGL.InternalFormat.R16;
            case PixelFormat.R16_SNorm:
                return Silk.NET.OpenGL.InternalFormat.R16SNorm;
            case PixelFormat.R16_UInt:
                return Silk.NET.OpenGL.InternalFormat.R16ui;
            case PixelFormat.R16_SInt:
                return Silk.NET.OpenGL.InternalFormat.R16i;
            case PixelFormat.R16_Float:
                return Silk.NET.OpenGL.InternalFormat.R16f;
            case PixelFormat.R32_UInt:
                return Silk.NET.OpenGL.InternalFormat.R32ui;
            case PixelFormat.R32_SInt:
                return Silk.NET.OpenGL.InternalFormat.R32i;
            case PixelFormat.R32_Float:
                return Silk.NET.OpenGL.InternalFormat.R32f;

            case PixelFormat.R8_G8_UNorm:
                return Silk.NET.OpenGL.InternalFormat.RG8;
            case PixelFormat.R8_G8_SNorm:
                return Silk.NET.OpenGL.InternalFormat.RG8SNorm;
            case PixelFormat.R8_G8_UInt:
                return Silk.NET.OpenGL.InternalFormat.RG8ui;
            case PixelFormat.R8_G8_SInt:
                return Silk.NET.OpenGL.InternalFormat.RG8i;

            case PixelFormat.R16_G16_UNorm:
                return Silk.NET.OpenGL.InternalFormat.RG16;
            case PixelFormat.R16_G16_SNorm:
                return Silk.NET.OpenGL.InternalFormat.RG16SNorm;
            case PixelFormat.R16_G16_UInt:
                return Silk.NET.OpenGL.InternalFormat.RG16ui;
            case PixelFormat.R16_G16_SInt:
                return Silk.NET.OpenGL.InternalFormat.RG16i;
            case PixelFormat.R16_G16_Float:
                return Silk.NET.OpenGL.InternalFormat.RG16f;

            case PixelFormat.R32_G32_UInt:
                return Silk.NET.OpenGL.InternalFormat.RG32ui;
            case PixelFormat.R32_G32_SInt:
                return Silk.NET.OpenGL.InternalFormat.RG32i;
            case PixelFormat.R32_G32_Float:
                return Silk.NET.OpenGL.InternalFormat.RG32f;

            case PixelFormat.R8_G8_B8_A8_UNorm:
                return Silk.NET.OpenGL.InternalFormat.Rgba8;
            case PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.Srgb8Alpha8;
            case PixelFormat.R8_G8_B8_A8_SNorm:
                return Silk.NET.OpenGL.InternalFormat.Rgba8SNorm;
            case PixelFormat.R8_G8_B8_A8_UInt:
                return Silk.NET.OpenGL.InternalFormat.Rgba8ui;
            case PixelFormat.R8_G8_B8_A8_SInt:
                return Silk.NET.OpenGL.InternalFormat.Rgba8i;

            case PixelFormat.R16_G16_B16_A16_UNorm:
                return Silk.NET.OpenGL.InternalFormat.Rgba16;
            case PixelFormat.R16_G16_B16_A16_SNorm:
                return Silk.NET.OpenGL.InternalFormat.Rgba16SNorm;
            case PixelFormat.R16_G16_B16_A16_UInt:
                return Silk.NET.OpenGL.InternalFormat.Rgba16ui;
            case PixelFormat.R16_G16_B16_A16_SInt:
                return Silk.NET.OpenGL.InternalFormat.Rgba16i;
            case PixelFormat.R16_G16_B16_A16_Float:
                return Silk.NET.OpenGL.InternalFormat.Rgba16f;

            case PixelFormat.R32_G32_B32_A32_Float:
                return Silk.NET.OpenGL.InternalFormat.Rgba32f;
            case PixelFormat.R32_G32_B32_A32_UInt:
                return Silk.NET.OpenGL.InternalFormat.Rgba32ui;
            case PixelFormat.R32_G32_B32_A32_SInt:
                return Silk.NET.OpenGL.InternalFormat.Rgba32i;

            case PixelFormat.B8_G8_R8_A8_UNorm:
                return Silk.NET.OpenGL.InternalFormat.Rgba;
            case PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.Srgb8Alpha8;

            case PixelFormat.BC1_Rgb_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgbS3TCDxt1Ext;
            case PixelFormat.BC1_Rgb_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.CompressedSrgbS3TCDxt1Ext;
            case PixelFormat.BC1_Rgba_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgbaS3TCDxt1Ext;
            case PixelFormat.BC1_Rgba_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.CompressedSrgbAlphaS3TCDxt1Ext;
            case PixelFormat.BC2_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgbaS3TCDxt3Ext;
            case PixelFormat.BC2_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.CompressedSrgbAlphaS3TCDxt3Ext;
            case PixelFormat.BC3_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgbaS3TCDxt5Ext;
            case PixelFormat.BC3_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.CompressedSrgbAlphaS3TCDxt5Ext;
            case PixelFormat.BC4_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRedRgtc1;
            case PixelFormat.BC4_SNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedSignedRedRgtc1;
            case PixelFormat.BC5_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRGRgtc2;
            case PixelFormat.BC5_SNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedSignedRGRgtc2;
            case PixelFormat.BC7_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgbaBptcUnorm;
            case PixelFormat.BC7_UNorm_SRgb:
                return Silk.NET.OpenGL.InternalFormat.CompressedSrgbAlphaBptcUnorm;

            case PixelFormat.ETC2_R8_G8_B8_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgb8Etc2;
            case PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgb8PunchthroughAlpha1Etc2;
            case PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                return Silk.NET.OpenGL.InternalFormat.CompressedRgba8Etc2Eac;

            case PixelFormat.D32_Float_S8_UInt:
                return Silk.NET.OpenGL.InternalFormat.Depth32fStencil8;
            case PixelFormat.D24_UNorm_S8_UInt:
                return Silk.NET.OpenGL.InternalFormat.Depth24Stencil8;

            case PixelFormat.R10_G10_B10_A2_UNorm:
                return Silk.NET.OpenGL.InternalFormat.Rgb10A2;
            case PixelFormat.R10_G10_B10_A2_UInt:
                return Silk.NET.OpenGL.InternalFormat.Rgb10A2ui;
            case PixelFormat.R11_G11_B10_Float:
                return Silk.NET.OpenGL.InternalFormat.R11fG11fB10f;

            default:
                throw new NotSupportedException(@$"Pixel format ""{format}"" is not supported.");
        }
    }
}
