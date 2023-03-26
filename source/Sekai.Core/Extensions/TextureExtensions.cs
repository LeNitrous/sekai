// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Textures;

namespace Sekai.Extensions;

internal static class TextureExtensions
{
    /// <summary>
    /// Converts the given texture kind as a Veldrid texture type.
    /// </summary>
    /// <param name="kind">The texture kind.</param>
    /// <returns>The Veldrid texture type.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument passed exceeds expected values.</exception>
    public static Veldrid.TextureType AsVeldridType(this TextureKind kind)
    {
        switch (kind)
        {
            case TextureKind.Texture1D:
                return Veldrid.TextureType.Texture1D;

            case TextureKind.Texture2D:
                return Veldrid.TextureType.Texture2D;

            case TextureKind.Texture3D:
                return Veldrid.TextureType.Texture3D;

            default:
                throw new ArgumentOutOfRangeException(nameof(kind));
        }
    }

    /// <summary>
    /// Converts the given texture usage as a Veldrid texture usage.
    /// </summary>
    /// <param name="usage">The texture usage.</param>
    /// <returns>The Veldrid texture usage.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument passed exceeds expected values.</exception>
    public static Veldrid.TextureUsage AsVeldridUsage(this TextureFlag usage)
    {
        switch (usage)
        {
            case TextureFlag.Sampled:
                return Veldrid.TextureUsage.Sampled;

            case TextureFlag.Storage:
                return Veldrid.TextureUsage.Storage;

            case TextureFlag.Cubemap:
                return Veldrid.TextureUsage.Cubemap;

            case TextureFlag.Sampled | TextureFlag.Cubemap:
                return Veldrid.TextureUsage.Sampled | Veldrid.TextureUsage.Cubemap;

            case TextureFlag.Storage | TextureFlag.Cubemap:
                return Veldrid.TextureUsage.Storage | Veldrid.TextureUsage.Cubemap;

            default:
                throw new ArgumentOutOfRangeException(nameof(usage));
        }
    }

    /// <summary>
    /// Converts a given pixel format to Veldrid.
    /// </summary>
    /// <param name="format">The pixel format.</param>
    /// <returns>A Veldrid-compatible pixel format.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the argument passed exceeds expected values.</exception>
    public static Veldrid.PixelFormat AsVeldridFormat(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.R8_UNorm:
                return Veldrid.PixelFormat.R8_UNorm;

            case PixelFormat.R8_SNorm:
                return Veldrid.PixelFormat.R8_SNorm;

            case PixelFormat.R8_UInt:
                return Veldrid.PixelFormat.R8_UInt;

            case PixelFormat.R8_SInt:
                return Veldrid.PixelFormat.R8_SInt;

            case PixelFormat.R8G8_UNorm:
                return Veldrid.PixelFormat.R8_G8_UNorm;

            case PixelFormat.R8G8_SNorm:
                return Veldrid.PixelFormat.R8_G8_SNorm;

            case PixelFormat.R8G8_UInt:
                return Veldrid.PixelFormat.R8_G8_UInt;

            case PixelFormat.R8G8_SInt:
                return Veldrid.PixelFormat.R8_G8_SInt;

            case PixelFormat.R8G8B8A8_UNorm:
                return Veldrid.PixelFormat.R8_G8_B8_A8_UNorm;

            case PixelFormat.R8G8B8A8_UNorm_SRgb:
                return Veldrid.PixelFormat.R8_G8_B8_A8_UNorm_SRgb;

            case PixelFormat.R8G8B8A8_SNorm:
                return Veldrid.PixelFormat.R8_G8_B8_A8_SNorm;

            case PixelFormat.R8G8B8A8_UInt:
                return Veldrid.PixelFormat.R8_G8_B8_A8_UInt;

            case PixelFormat.R8G8B8A8_SInt:
                return Veldrid.PixelFormat.R8_G8_B8_A8_SInt;

            case PixelFormat.B8G8R8A8_UNorm:
                return Veldrid.PixelFormat.B8_G8_R8_A8_UNorm;

            case PixelFormat.B8G8R8A8_UNorm_SRgb:
                return Veldrid.PixelFormat.B8_G8_R8_A8_UNorm_SRgb;

            case PixelFormat.R16_UNorm:
                return Veldrid.PixelFormat.R16_UNorm;

            case PixelFormat.R16_SNorm:
                return Veldrid.PixelFormat.R16_SNorm;

            case PixelFormat.R16_UInt:
                return Veldrid.PixelFormat.R16_UInt;

            case PixelFormat.R16_SInt:
                return Veldrid.PixelFormat.R16_SInt;

            case PixelFormat.R16_Float:
                return Veldrid.PixelFormat.R16_Float;

            case PixelFormat.R16G16_UNorm:
                return Veldrid.PixelFormat.R16_G16_UNorm;

            case PixelFormat.R16G16_SNorm:
                return Veldrid.PixelFormat.R16_G16_SNorm;

            case PixelFormat.R16G16_UInt:
                return Veldrid.PixelFormat.R16_G16_UInt;

            case PixelFormat.R16G16_SInt:
                return Veldrid.PixelFormat.R16_G16_SInt;

            case PixelFormat.R16G16_Float:
                return Veldrid.PixelFormat.R16_G16_Float;

            case PixelFormat.R16G16B16A16_UNorm:
                return Veldrid.PixelFormat.R16_G16_B16_A16_UNorm;

            case PixelFormat.R16G16B16A16_SNorm:
                return Veldrid.PixelFormat.R16_G16_B16_A16_SNorm;

            case PixelFormat.R16G16B16A16_UInt:
                return Veldrid.PixelFormat.R16_G16_B16_A16_UInt;

            case PixelFormat.R16G16B16A16_SInt:
                return Veldrid.PixelFormat.R16_G16_B16_A16_SInt;

            case PixelFormat.R16G16B16A16_Float:
                return Veldrid.PixelFormat.R16_G16_B16_A16_Float;

            case PixelFormat.R32_UInt:
                return Veldrid.PixelFormat.R32_UInt;

            case PixelFormat.R32_SInt:
                return Veldrid.PixelFormat.R32_SInt;

            case PixelFormat.R32_Float:
                return Veldrid.PixelFormat.R32_Float;

            case PixelFormat.R32G32_UInt:
                return Veldrid.PixelFormat.R32_G32_UInt;

            case PixelFormat.R32G32_SInt:
                return Veldrid.PixelFormat.R32_G32_SInt;

            case PixelFormat.R32G32_Float:
                return Veldrid.PixelFormat.R32_G32_Float;

            case PixelFormat.R32G32B32A32_UInt:
                return Veldrid.PixelFormat.R32_G32_B32_A32_UInt;

            case PixelFormat.R32G32B32A32_SInt:
                return Veldrid.PixelFormat.R32_G32_B32_A32_SInt;

            case PixelFormat.R32G32B32A32_Float:
                return Veldrid.PixelFormat.R32_G32_B32_A32_Float;

            case PixelFormat.D24_UNorm_S8_UInt:
                return Veldrid.PixelFormat.D24_UNorm_S8_UInt;

            case PixelFormat.D32_Float_S8_UInt:
                return Veldrid.PixelFormat.D32_Float_S8_UInt;

            case PixelFormat.BC1_Rgb_UNorm:
                return Veldrid.PixelFormat.BC1_Rgb_UNorm;

            case PixelFormat.BC1_Rgba_UNorm:
                return Veldrid.PixelFormat.BC1_Rgba_UNorm;

            case PixelFormat.BC1_Rgb_UNorm_SRgb:
                return Veldrid.PixelFormat.BC1_Rgb_UNorm_SRgb;

            case PixelFormat.BC1_Rgba_UNorm_SRgb:
                return Veldrid.PixelFormat.BC1_Rgba_UNorm_SRgb;

            case PixelFormat.BC2_UNorm:
                return Veldrid.PixelFormat.BC2_UNorm;

            case PixelFormat.BC2_UNorm_SRgb:
                return Veldrid.PixelFormat.BC2_UNorm_SRgb;

            case PixelFormat.BC3_UNorm:
                return Veldrid.PixelFormat.BC3_UNorm;

            case PixelFormat.BC3_UNorm_SRgb:
                return Veldrid.PixelFormat.BC3_UNorm_SRgb;

            case PixelFormat.BC4_UNorm:
                return Veldrid.PixelFormat.BC4_UNorm;

            case PixelFormat.BC4_SNorm:
                return Veldrid.PixelFormat.BC4_SNorm;

            case PixelFormat.BC5_UNorm:
                return Veldrid.PixelFormat.BC5_UNorm;

            case PixelFormat.BC5_SNorm:
                return Veldrid.PixelFormat.BC5_SNorm;

            case PixelFormat.BC7_UNorm:
                return Veldrid.PixelFormat.BC7_UNorm;

            case PixelFormat.BC7_UNorm_SRgb:
                return Veldrid.PixelFormat.BC7_UNorm_SRgb;

            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }

    /// <summary>
    /// Gets whether the given format is a depth-stencil format.
    /// </summary>
    /// <param name="format">The format to verify.</param>
    /// <returns><see langword="true"/> if it is a depth stencil format. Otherwise, returns <see langword="false"/>.</returns>
    public static bool IsDepthStencil(this PixelFormat format)
    {
        switch (format)
        {
            case PixelFormat.D24_UNorm_S8_UInt:
            case PixelFormat.D32_Float_S8_UInt:
                return true;

            default:
                return false;
        }
    }
}
