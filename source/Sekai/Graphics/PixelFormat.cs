// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Runtime.CompilerServices;

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of pixel formats.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// RGBA component order. Each component is an 8-bit unsigned normalized integer.
    /// </summary>
    R8G8B8A8_UNorm,

    /// <summary>
    /// BGRA component order. Each component is an 8-bit unsigned normalized integer.
    /// </summary>
    B8G8R8A8_UNorm,

    /// <summary>
    /// Single-channel, 8-bit unsigned normalized integer.
    /// </summary>
    R8_UNorm,

    /// <summary>
    /// Single-channel, 16-bit unsigned normalized integer. Can be used as a depth format.
    /// </summary>
    R16_UNorm,

    /// <summary>
    /// RGBA component order. Each component is a 32-bit signed floating-point value.
    /// </summary>
    R32G32B32A32_Float,

    /// <summary>
    /// Single-channel, 32-bit signed floating-point value. Can be used as a depth format.
    /// </summary>
    R32_Float,

    /// <summary>
    /// BC3 block compressed format.
    /// </summary>
    BC3_UNorm,

    /// <summary>
    /// A depth-stencil format where the depth is stored in a 24-bit unsigned normalized integer, and the stencil is stored
    /// in an 8-bit unsigned integer.
    /// </summary>
    D24_UNorm_S8_UInt,

    /// <summary>
    /// A depth-stencil format where the depth is stored in a 32-bit signed floating-point value, and the stencil is stored
    /// in an 8-bit unsigned integer.
    /// </summary>
    D32_Float_S8_UInt,

    /// <summary>
    /// RGBA component order. Each component is a 32-bit unsigned integer.
    /// </summary>
    R32G32B32A32_UInt,

    /// <summary>
    /// RG component order. Each component is an 8-bit signed normalized integer.
    /// </summary>
    R8G8_SNorm,

    /// <summary>
    /// BC1 block compressed format with no alpha.
    /// </summary>
    BC1_RGB_UNorm,

    /// <summary>
    /// BC1 block compressed format with a single-bit alpha channel.
    /// </summary>
    BC1_RGBA_UNorm,

    /// <summary>
    /// BC2 block compressed format.
    /// </summary>
    BC2_UNorm,

    /// <summary>
    /// A 32-bit packed format. The 10-bit R component occupies bits 0..9, the 10-bit G component occupies bits 10..19,
    /// the 10-bit A component occupies 20..29, and the 2-bit A component occupies bits 30..31. Each value is an unsigned,
    /// normalized integer.
    /// </summary>
    R10G10B10A2_UNorm,

    /// <summary>
    /// A 32-bit packed format. The 10-bit R component occupies bits 0..9, the 10-bit G component occupies bits 10..19,
    /// the 10-bit A component occupies 20..29, and the 2-bit A component occupies bits 30..31. Each value is an unsigned
    /// integer.
    /// </summary>
    R10G10B10A2_UInt,

    /// <summary>
    /// A 32-bit packed format. The 11-bit R componnent occupies bits 0..10, the 11-bit G component occupies bits 11..21,
    /// and the 10-bit B component occupies bits 22..31. Each value is an unsigned floating point value.
    /// </summary>
    R11G11B10_Float,

    /// <summary>
    /// Single-channel, 8-bit signed normalized integer.
    /// </summary>
    R8_SNorm,

    /// <summary>
    /// Single-channel, 8-bit unsigned integer.
    /// </summary>
    R8_UInt,

    /// <summary>
    /// Single-channel, 8-bit signed integer.
    /// </summary>
    R8_SInt,

    /// <summary>
    /// Single-channel, 16-bit signed normalized integer.
    /// </summary>
    R16_SNorm,

    /// <summary>
    /// Single-channel, 16-bit unsigned integer.
    /// </summary>
    R16_UInt,

    /// <summary>
    /// Single-channel, 16-bit signed integer.
    /// </summary>
    R16_SInt,

    /// <summary>
    /// Single-channel, 16-bit signed floating-point value.
    /// </summary>
    R16_Float,

    /// <summary>
    /// Single-channel, 32-bit unsigned integer
    /// </summary>
    R32_UInt,

    /// <summary>
    /// Single-channel, 32-bit signed integer
    /// </summary>
    R32_SInt,

    /// <summary>
    /// RG component order. Each component is an 8-bit unsigned normalized integer.
    /// </summary>
    R8G8_UNorm,

    /// <summary>
    /// RG component order. Each component is an 8-bit unsigned integer.
    /// </summary>
    R8G8_UInt,

    /// <summary>
    /// RG component order. Each component is an 8-bit signed integer.
    /// </summary>
    R8G8_SInt,

    /// <summary>
    /// RG component order. Each component is a 16-bit unsigned normalized integer.
    /// </summary>
    R16G16_UNorm,

    /// <summary>
    /// RG component order. Each component is a 16-bit signed normalized integer.
    /// </summary>
    R16G16_SNorm,

    /// <summary>
    /// RG component order. Each component is a 16-bit unsigned integer.
    /// </summary>
    R16G16_UInt,

    /// <summary>
    /// RG component order. Each component is a 16-bit signed integer.
    /// </summary>
    R16G16_SInt,

    /// <summary>
    /// RG component order. Each component is a 16-bit signed floating-point value.
    /// </summary>
    R16G16_Float,

    /// <summary>
    /// RG component order. Each component is a 32-bit unsigned integer.
    /// </summary>
    R32G32_UInt,

    /// <summary>
    /// RG component order. Each component is a 32-bit signed integer.
    /// </summary>
    R32G32_SInt,

    /// <summary>
    /// RG component order. Each component is a 32-bit signed floating-point value.
    /// </summary>
    R32G32_Float,

    /// <summary>
    /// RGBA component order. Each component is an 8-bit signed normalized integer.
    /// </summary>
    R8G8B8A8_SNorm,

    /// <summary>
    /// RGBA component order. Each component is an 8-bit unsigned integer.
    /// </summary>
    R8G8B8A8_UInt,

    /// <summary>
    /// RGBA component order. Each component is an 8-bit signed integer.
    /// </summary>
    R8G8B8A8_SInt,

    /// <summary>
    /// RGBA component order. Each component is a 16-bit unsigned normalized integer.
    /// </summary>
    R16G16B16A16_UNorm,

    /// <summary>
    /// RGBA component order. Each component is a 16-bit signed normalized integer.
    /// </summary>
    R16G16B16A16_SNorm,

    /// <summary>
    /// RGBA component order. Each component is a 16-bit unsigned integer.
    /// </summary>
    R16G16B16A16_UInt,

    /// <summary>
    /// RGBA component order. Each component is a 16-bit signed integer.
    /// </summary>
    R16G16B16A16_SInt,

    /// <summary>
    /// RGBA component order. Each component is a 16-bit floating-point value.
    /// </summary>
    R16G16B16A16_Float,

    /// <summary>
    /// RGBA component order. Each component is a 32-bit signed integer.
    /// </summary>
    R32G32B32A32_SInt,

    /// <summary>
    /// A 64-bit, 4x4 block-compressed format storing unsigned normalized RGB data.
    /// </summary>
    ETC2_R8G8B8_UNorm,

    /// <summary>
    /// A 64-bit, 4x4 block-compressed format storing unsigned normalized RGB data, as well as 1 bit of alpha data.
    /// </summary>
    ETC2_R8G8B8A1_UNorm,

    /// <summary>
    /// A 128-bit, 4x4 block-compressed format storing 64 bits of unsigned normalized RGB data, as well as 64 bits of alpha
    /// data.
    /// </summary>
    ETC2_R8G8B8A8_UNorm,

    /// <summary>
    /// BC4 block compressed format, unsigned normalized values.
    /// </summary>
    BC4_UNorm,

    /// <summary>
    /// BC4 block compressed format, signed normalized values.
    /// </summary>
    BC4_SNorm,

    /// <summary>
    /// BC5 block compressed format, unsigned normalized values.
    /// </summary>
    BC5_UNorm,

    /// <summary>
    /// BC5 block compressed format, signed normalized values.
    /// </summary>
    BC5_SNorm,

    /// <summary>
    /// BC7 block compressed format.
    /// </summary>
    BC7_UNorm,

    /// <summary>
    /// RGBA component order. Each component is an 8-bit unsigned normalized integer.
    /// This is an sRGB format.
    /// </summary>
    R8G8B8A8_UNorm_SRgb,

    /// <summary>
    /// BGRA component order. Each component is an 8-bit unsigned normalized integer.
    /// This is an sRGB format.
    /// </summary>
    B8G8R8A8_UNorm_SRgb,

    /// <summary>
    /// BC1 block compressed format with no alpha.
    /// This is an sRGB format.
    /// </summary>
    BC1_RGB_UNorm_SRgb,

    /// <summary>
    /// BC1 block compressed format with a single-bit alpha channel.
    /// This is an sRGB format.
    /// </summary>
    BC1_RGBA_UNorm_SRgb,

    /// <summary>
    /// BC2 block compressed format.
    /// This is an sRGB format.
    /// </summary>
    BC2_UNorm_SRgb,

    /// <summary>
    /// BC3 block compressed format.
    /// This is an sRGB format.
    /// </summary>
    BC3_UNorm_SRgb,

    /// <summary>
    /// BC7 block compressed format.
    /// This is an sRGB format.
    /// </summary>
    BC7_UNorm_SRgb,
}

public static class PixelFormatExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SizeOfFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.R8_UNorm:
            case PixelFormat.R8_SNorm:
            case PixelFormat.R8_UInt:
            case PixelFormat.R8_SInt:
                return 1;

            case PixelFormat.R16_UNorm:
            case PixelFormat.R16_SNorm:
            case PixelFormat.R16_UInt:
            case PixelFormat.R16_SInt:
            case PixelFormat.R16_Float:
            case PixelFormat.R8G8_UNorm:
            case PixelFormat.R8G8_SNorm:
            case PixelFormat.R8G8_UInt:
            case PixelFormat.R8G8_SInt:
                return 2;

            case PixelFormat.R32_UInt:
            case PixelFormat.R32_SInt:
            case PixelFormat.R32_Float:
            case PixelFormat.R16G16_UNorm:
            case PixelFormat.R16G16_SNorm:
            case PixelFormat.R16G16_UInt:
            case PixelFormat.R16G16_SInt:
            case PixelFormat.R16G16_Float:
            case PixelFormat.R8G8B8A8_UNorm:
            case PixelFormat.R8G8B8A8_UNorm_SRgb:
            case PixelFormat.R8G8B8A8_SNorm:
            case PixelFormat.R8G8B8A8_UInt:
            case PixelFormat.R8G8B8A8_SInt:
            case PixelFormat.B8G8R8A8_UNorm:
            case PixelFormat.B8G8R8A8_UNorm_SRgb:
            case PixelFormat.R10G10B10A2_UNorm:
            case PixelFormat.R10G10B10A2_UInt:
            case PixelFormat.R11G11B10_Float:
            case PixelFormat.D24_UNorm_S8_UInt:
                return 4;

            case PixelFormat.D32_Float_S8_UInt:
                return 5;

            case PixelFormat.R16G16B16A16_UNorm:
            case PixelFormat.R16G16B16A16_SNorm:
            case PixelFormat.R16G16B16A16_UInt:
            case PixelFormat.R16G16B16A16_SInt:
            case PixelFormat.R16G16B16A16_Float:
            case PixelFormat.R32G32_UInt:
            case PixelFormat.R32G32_SInt:
            case PixelFormat.R32G32_Float:
                return 8;

            case PixelFormat.R32G32B32A32_Float:
            case PixelFormat.R32G32B32A32_UInt:
            case PixelFormat.R32G32B32A32_SInt:
                return 16;

            case PixelFormat.BC1_RGB_UNorm:
            case PixelFormat.BC1_RGB_UNorm_SRgb:
            case PixelFormat.BC1_RGBA_UNorm:
            case PixelFormat.BC1_RGBA_UNorm_SRgb:
            case PixelFormat.BC2_UNorm:
            case PixelFormat.BC2_UNorm_SRgb:
            case PixelFormat.BC3_UNorm:
            case PixelFormat.BC3_UNorm_SRgb:
            case PixelFormat.BC4_UNorm:
            case PixelFormat.BC4_SNorm:
            case PixelFormat.BC5_UNorm:
            case PixelFormat.BC5_SNorm:
            case PixelFormat.BC7_UNorm:
            case PixelFormat.BC7_UNorm_SRgb:
            case PixelFormat.ETC2_R8G8B8_UNorm:
            case PixelFormat.ETC2_R8G8B8A1_UNorm:
            case PixelFormat.ETC2_R8G8B8A8_UNorm:
                throw new ArgumentException("Unable to determine size of a compressed format.", nameof(format));

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetBlockSize(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.BC1_RGB_UNorm:
            case PixelFormat.BC1_RGB_UNorm_SRgb:
            case PixelFormat.BC1_RGBA_UNorm:
            case PixelFormat.BC1_RGBA_UNorm_SRgb:
            case PixelFormat.BC4_UNorm:
            case PixelFormat.BC4_SNorm:
            case PixelFormat.ETC2_R8G8B8_UNorm:
            case PixelFormat.ETC2_R8G8B8A1_UNorm:
                return 8;

            case PixelFormat.BC2_UNorm:
            case PixelFormat.BC2_UNorm_SRgb:
            case PixelFormat.BC3_UNorm:
            case PixelFormat.BC3_UNorm_SRgb:
            case PixelFormat.BC5_UNorm:
            case PixelFormat.BC5_SNorm:
            case PixelFormat.BC7_UNorm:
            case PixelFormat.BC7_UNorm_SRgb:
            case PixelFormat.ETC2_R8G8B8A8_UNorm:
                return 16;

            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRowCount(this PixelFormat format, int height)
    {
        switch (format)
        {
            case PixelFormat.BC1_RGB_UNorm:
            case PixelFormat.BC1_RGB_UNorm_SRgb:
            case PixelFormat.BC1_RGBA_UNorm:
            case PixelFormat.BC1_RGBA_UNorm_SRgb:
            case PixelFormat.BC2_UNorm:
            case PixelFormat.BC2_UNorm_SRgb:
            case PixelFormat.BC3_UNorm:
            case PixelFormat.BC3_UNorm_SRgb:
            case PixelFormat.BC4_UNorm:
            case PixelFormat.BC4_SNorm:
            case PixelFormat.BC5_UNorm:
            case PixelFormat.BC5_SNorm:
            case PixelFormat.BC7_UNorm:
            case PixelFormat.BC7_UNorm_SRgb:
            case PixelFormat.ETC2_R8G8B8_UNorm:
            case PixelFormat.ETC2_R8G8B8A1_UNorm:
            case PixelFormat.ETC2_R8G8B8A8_UNorm:
                return (height + 3) / 4;

            default:
                return height;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetRowPitch(this PixelFormat format, int width)
    {
        switch (format)
        {
            case PixelFormat.BC1_RGB_UNorm:
            case PixelFormat.BC1_RGB_UNorm_SRgb:
            case PixelFormat.BC1_RGBA_UNorm:
            case PixelFormat.BC1_RGBA_UNorm_SRgb:
            case PixelFormat.BC2_UNorm:
            case PixelFormat.BC2_UNorm_SRgb:
            case PixelFormat.BC3_UNorm:
            case PixelFormat.BC3_UNorm_SRgb:
            case PixelFormat.BC4_UNorm:
            case PixelFormat.BC4_SNorm:
            case PixelFormat.BC5_UNorm:
            case PixelFormat.BC5_SNorm:
            case PixelFormat.BC7_UNorm:
            case PixelFormat.BC7_UNorm_SRgb:
            case PixelFormat.ETC2_R8G8B8_UNorm:
            case PixelFormat.ETC2_R8G8B8A1_UNorm:
            case PixelFormat.ETC2_R8G8B8A8_UNorm:
                return (width + 3) / 4 * format.GetBlockSize();

            default:
                return width * format.SizeOfFormat();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GetDepthPitch(this PixelFormat format, int width, int height)
    {
        return format.GetRowPitch(width) * format.GetRowCount(height);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStencil(this PixelFormat format)
    {
        return format is PixelFormat.D24_UNorm_S8_UInt or PixelFormat.D32_Float_S8_UInt;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDepthStencil(this PixelFormat format)
    {
        return format is PixelFormat.D32_Float_S8_UInt or PixelFormat.D24_UNorm_S8_UInt or PixelFormat.R16_UNorm or PixelFormat.R32_Float;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCompressed(this PixelFormat format)
    {
        return format is PixelFormat.BC1_RGB_UNorm
                or PixelFormat.BC1_RGB_UNorm_SRgb
                or PixelFormat.BC1_RGBA_UNorm
                or PixelFormat.BC1_RGBA_UNorm_SRgb
                or PixelFormat.BC2_UNorm
                or PixelFormat.BC2_UNorm_SRgb
                or PixelFormat.BC3_UNorm
                or PixelFormat.BC3_UNorm_SRgb
                or PixelFormat.BC4_UNorm
                or PixelFormat.BC4_SNorm
                or PixelFormat.BC5_UNorm
                or PixelFormat.BC5_SNorm
                or PixelFormat.BC7_UNorm
                or PixelFormat.BC7_UNorm_SRgb
                or PixelFormat.ETC2_R8G8B8_UNorm
                or PixelFormat.ETC2_R8G8B8A1_UNorm
                or PixelFormat.ETC2_R8G8B8A8_UNorm;
    }
}
