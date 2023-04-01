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

            case PixelFormat.BC1_RGB_UNorm:
                return Veldrid.PixelFormat.BC1_Rgb_UNorm;

            case PixelFormat.BC1_Rgba_UNorm:
                return Veldrid.PixelFormat.BC1_Rgba_UNorm;

            case PixelFormat.BC1_Rgb_UNorm_SRgb:
                return Veldrid.PixelFormat.BC1_Rgb_UNorm_SRgb;

            case PixelFormat.BC1_RGBA_UNorm_SRgb:
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

            case PixelFormat.R10B10G10A2_UInt:
                return Veldrid.PixelFormat.R10_G10_B10_A2_UInt;

            case PixelFormat.R10B10G10A2_UNorm:
                return Veldrid.PixelFormat.R10_G10_B10_A2_UNorm;

            case PixelFormat.R11G11B10_Float:
                return Veldrid.PixelFormat.R11_G11_B10_Float;

            case PixelFormat.ETC2_R8G8B8_UNorm:
                return Veldrid.PixelFormat.ETC2_R8_G8_B8_UNorm;

            case PixelFormat.ETC2_R8G8B8A1_UNorm:
                return Veldrid.PixelFormat.ETC2_R8_G8_B8_A1_UNorm;

            case PixelFormat.ETC2_R8G8B8A8_UNorm:
                return Veldrid.PixelFormat.ETC2_R8_G8_B8_A8_UNorm;

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

    /// <summary>
    /// Gets the addressing mode as a sampler address mode.
    /// </summary>
    /// <param name="mode">The addressing mode.</param>
    /// <returns>A Veldrid sampler address mode.</returns>
    /// <exception cref="NotSupportedException">Thrown when the value passed is not supported.</exception>
    public static Veldrid.SamplerAddressMode AsAddressMode(this TextureAddressMode mode)
    {
        switch (mode)
        {
            case TextureAddressMode.Wrap:
                return Veldrid.SamplerAddressMode.Wrap;

            case TextureAddressMode.Clamp:
                return Veldrid.SamplerAddressMode.Clamp;

            case TextureAddressMode.Border:
                return Veldrid.SamplerAddressMode.Border;

            case TextureAddressMode.Mirror:
                return Veldrid.SamplerAddressMode.Mirror;

            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Gets the color as a sampler border color.
    /// </summary>
    /// <param name="color">The border color.</param>
    /// <returns>A Veldrid sampler border color.</returns>
    /// <exception cref="NotSupportedException">Thrown when the value passed is not supported.</exception>
    public static Veldrid.SamplerBorderColor AsVeldridBorder(this TextureBorderColor color)
    {
        switch (color)
        {
            case TextureBorderColor.Transparent:
                return Veldrid.SamplerBorderColor.TransparentBlack;

            case TextureBorderColor.Black:
                return Veldrid.SamplerBorderColor.OpaqueBlack;

            case TextureBorderColor.White:
                return Veldrid.SamplerBorderColor.OpaqueWhite;

            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Gets the filtering mode as a sampler filter.
    /// </summary>
    /// <param name="mode">The filtering mode.</param>
    /// <returns>A Veldrid sampler filter.</returns>
    /// <exception cref="NotSupportedException">Thrown when the value passed is not supported.</exception>
    public static Veldrid.SamplerFilter AsVeldridFilter(this TextureFilteringMode mode)
    {
        switch (mode)
        {
            case TextureFilteringMode.MinPoint | TextureFilteringMode.MagPoint | TextureFilteringMode.MipPoint:
                return Veldrid.SamplerFilter.MinPoint_MagPoint_MipPoint;

            case TextureFilteringMode.MinLinear | TextureFilteringMode.MagPoint | TextureFilteringMode.MipPoint:
                return Veldrid.SamplerFilter.MinLinear_MagPoint_MipPoint;

            case TextureFilteringMode.MinPoint | TextureFilteringMode.MagLinear | TextureFilteringMode.MipPoint:
                return Veldrid.SamplerFilter.MinPoint_MagLinear_MipPoint;

            case TextureFilteringMode.MinPoint | TextureFilteringMode.MagPoint | TextureFilteringMode.MipLinear:
                return Veldrid.SamplerFilter.MinPoint_MagPoint_MipLinear;

            case TextureFilteringMode.MinLinear | TextureFilteringMode.MagLinear | TextureFilteringMode.MipPoint:
                return Veldrid.SamplerFilter.MinLinear_MagLinear_MipPoint;

            case TextureFilteringMode.MinPoint | TextureFilteringMode.MagLinear | TextureFilteringMode.MipLinear:
                return Veldrid.SamplerFilter.MinPoint_MagLinear_MipLinear;

            case TextureFilteringMode.MinLinear | TextureFilteringMode.MagPoint | TextureFilteringMode.MipLinear:
                return Veldrid.SamplerFilter.MinLinear_MagPoint_MipLinear;

            case TextureFilteringMode.MinLinear | TextureFilteringMode.MagLinear | TextureFilteringMode.MipLinear:
                return Veldrid.SamplerFilter.MinLinear_MagLinear_MipLinear;

            case TextureFilteringMode.Anisotropic:
                return Veldrid.SamplerFilter.Anisotropic;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }
    }

    public static TextureAddressMode AsAddressMode(this Veldrid.SamplerAddressMode mode)
    {
        switch (mode)
        {
            case Veldrid.SamplerAddressMode.Wrap:
                return TextureAddressMode.Wrap;

            case Veldrid.SamplerAddressMode.Mirror:
                return TextureAddressMode.Mirror;

            case Veldrid.SamplerAddressMode.Clamp:
                return TextureAddressMode.Clamp;

            case Veldrid.SamplerAddressMode.Border:
                return TextureAddressMode.Border;

            default:
                throw new ArgumentOutOfRangeException(nameof(mode));
        }
    }

    public static TextureBorderColor AsBorderColor(this Veldrid.SamplerBorderColor color)
    {
        switch (color)
        {
            case Veldrid.SamplerBorderColor.TransparentBlack:
                return TextureBorderColor.Transparent;

            case Veldrid.SamplerBorderColor.OpaqueBlack:
                return TextureBorderColor.Black;

            case Veldrid.SamplerBorderColor.OpaqueWhite:
                return TextureBorderColor.White;

            default:
                throw new ArgumentOutOfRangeException(nameof(color));
        }
    }

    public static TextureFilteringMode AsFilteringMode(this Veldrid.SamplerFilter filter)
    {
        switch (filter)
        {
            case Veldrid.SamplerFilter.MinPoint_MagPoint_MipPoint:
                return TextureFilteringMode.MinPoint | TextureFilteringMode.MagPoint | TextureFilteringMode.MipPoint;

            case Veldrid.SamplerFilter.MinPoint_MagPoint_MipLinear:
                return TextureFilteringMode.MinPoint | TextureFilteringMode.MagPoint | TextureFilteringMode.MipLinear;

            case Veldrid.SamplerFilter.MinPoint_MagLinear_MipPoint:
                return TextureFilteringMode.MinPoint | TextureFilteringMode.MagLinear | TextureFilteringMode.MipPoint;

            case Veldrid.SamplerFilter.MinPoint_MagLinear_MipLinear:
                return TextureFilteringMode.MinPoint | TextureFilteringMode.MagLinear | TextureFilteringMode.MipLinear;

            case Veldrid.SamplerFilter.MinLinear_MagPoint_MipPoint:
                return TextureFilteringMode.MinLinear | TextureFilteringMode.MagPoint | TextureFilteringMode.MipPoint;

            case Veldrid.SamplerFilter.MinLinear_MagPoint_MipLinear:
                return TextureFilteringMode.MinLinear | TextureFilteringMode.MagPoint | TextureFilteringMode.MipLinear;

            case Veldrid.SamplerFilter.MinLinear_MagLinear_MipPoint:
                return TextureFilteringMode.MinLinear | TextureFilteringMode.MagLinear | TextureFilteringMode.MipPoint;

            case Veldrid.SamplerFilter.MinLinear_MagLinear_MipLinear:
                return TextureFilteringMode.MinLinear | TextureFilteringMode.MagLinear | TextureFilteringMode.MipLinear;

            case Veldrid.SamplerFilter.Anisotropic:
                return TextureFilteringMode.Anisotropic;

            default:
                throw new ArgumentOutOfRangeException(nameof(filter));
        }
    }

    public static TextureFlag AsFlags(this Veldrid.TextureUsage usage)
    {
        switch (usage)
        {
            case Veldrid.TextureUsage.Sampled:
                return TextureFlag.Sampled;

            case Veldrid.TextureUsage.Storage:
                return TextureFlag.Storage;

            case Veldrid.TextureUsage.Cubemap:
                return TextureFlag.Cubemap;

            case Veldrid.TextureUsage.Sampled | Veldrid.TextureUsage.Cubemap:
                return TextureFlag.Sampled | TextureFlag.Cubemap;

            case Veldrid.TextureUsage.Storage | Veldrid.TextureUsage.Cubemap:
                return TextureFlag.Storage | TextureFlag.Cubemap;

            default:
                throw new ArgumentOutOfRangeException(nameof(usage));
        }
    }

    public static TextureKind AsKind(this Veldrid.TextureType type)
    {
        switch (type)
        {
            case Veldrid.TextureType.Texture1D:
                return TextureKind.Texture1D;

            case Veldrid.TextureType.Texture2D:
                return TextureKind.Texture2D;

            case Veldrid.TextureType.Texture3D:
                return TextureKind.Texture3D;

            default:
                throw new ArgumentOutOfRangeException(nameof(type));
        }
    }

    public static PixelFormat AsPixelFormat(this Veldrid.PixelFormat format)
    {
        switch (format)
        {
            case Veldrid.PixelFormat.R8_G8_B8_A8_UNorm:
                return PixelFormat.R8G8B8A8_UNorm;

            case Veldrid.PixelFormat.B8_G8_R8_A8_UNorm:
                return PixelFormat.B8G8R8A8_UNorm;

            case Veldrid.PixelFormat.R8_UNorm:
                return PixelFormat.R8_UNorm;

            case Veldrid.PixelFormat.R16_UNorm:
                return PixelFormat.R16_UNorm;

            case Veldrid.PixelFormat.R32_G32_B32_A32_Float:
                return PixelFormat.R32G32B32A32_Float;

            case Veldrid.PixelFormat.R32_Float:
                return PixelFormat.R32_Float;

            case Veldrid.PixelFormat.BC3_UNorm:
                return PixelFormat.BC3_UNorm;

            case Veldrid.PixelFormat.D24_UNorm_S8_UInt:
                return PixelFormat.D24_UNorm_S8_UInt;

            case Veldrid.PixelFormat.D32_Float_S8_UInt:
                return PixelFormat.D32_Float_S8_UInt;

            case Veldrid.PixelFormat.R32_G32_B32_A32_UInt:
                return PixelFormat.R32G32B32A32_UInt;

            case Veldrid.PixelFormat.R8_G8_SNorm:
                return PixelFormat.R8G8_SNorm;

            case Veldrid.PixelFormat.BC1_Rgb_UNorm:
                return PixelFormat.BC1_RGB_UNorm;

            case Veldrid.PixelFormat.BC1_Rgba_UNorm:
                return PixelFormat.BC1_Rgba_UNorm;

            case Veldrid.PixelFormat.BC2_UNorm:
                return PixelFormat.BC2_UNorm;

            case Veldrid.PixelFormat.R10_G10_B10_A2_UNorm:
                return PixelFormat.R10B10G10A2_UNorm;

            case Veldrid.PixelFormat.R10_G10_B10_A2_UInt:
                return PixelFormat.R10B10G10A2_UInt;

            case Veldrid.PixelFormat.R11_G11_B10_Float:
                return PixelFormat.R11G11B10_Float;

            case Veldrid.PixelFormat.R8_SNorm:
                return PixelFormat.R8_SNorm;

            case Veldrid.PixelFormat.R8_UInt:
                return PixelFormat.R8_UInt;

            case Veldrid.PixelFormat.R8_SInt:
                return PixelFormat.R8_SInt;

            case Veldrid.PixelFormat.R16_SNorm:
                return PixelFormat.R16_SNorm;

            case Veldrid.PixelFormat.R16_UInt:
                return PixelFormat.R16_UInt;

            case Veldrid.PixelFormat.R16_SInt:
                return PixelFormat.R16_SInt;

            case Veldrid.PixelFormat.R16_Float:
                return PixelFormat.R16_Float;

            case Veldrid.PixelFormat.R32_UInt:
                return PixelFormat.R32_UInt;

            case Veldrid.PixelFormat.R32_SInt:
                return PixelFormat.R32_SInt;

            case Veldrid.PixelFormat.R8_G8_UNorm:
                return PixelFormat.R8G8_UNorm;

            case Veldrid.PixelFormat.R8_G8_UInt:
                return PixelFormat.R8G8_UInt;

            case Veldrid.PixelFormat.R8_G8_SInt:
                return PixelFormat.R8G8_SInt;

            case Veldrid.PixelFormat.R16_G16_UNorm:
                return PixelFormat.R16G16_UNorm;

            case Veldrid.PixelFormat.R16_G16_SNorm:
                return PixelFormat.R16G16_SNorm;

            case Veldrid.PixelFormat.R16_G16_UInt:
                return PixelFormat.R16G16_UInt;

            case Veldrid.PixelFormat.R16_G16_SInt:
                return PixelFormat.R16G16_SInt;

            case Veldrid.PixelFormat.R16_G16_Float:
                return PixelFormat.R16G16_Float;

            case Veldrid.PixelFormat.R32_G32_UInt:
                return PixelFormat.R32G32_UInt;

            case Veldrid.PixelFormat.R32_G32_SInt:
                return PixelFormat.R32G32_SInt;

            case Veldrid.PixelFormat.R32_G32_Float:
                return PixelFormat.R32G32_Float;

            case Veldrid.PixelFormat.R8_G8_B8_A8_SNorm:
                return PixelFormat.R8G8B8A8_SNorm;

            case Veldrid.PixelFormat.R8_G8_B8_A8_UInt:
                return PixelFormat.R8G8B8A8_UInt;

            case Veldrid.PixelFormat.R8_G8_B8_A8_SInt:
                return PixelFormat.R8G8B8A8_SInt;

            case Veldrid.PixelFormat.R16_G16_B16_A16_UNorm:
                return PixelFormat.R16G16B16A16_UNorm;

            case Veldrid.PixelFormat.R16_G16_B16_A16_SNorm:
                return PixelFormat.R16G16B16A16_SNorm;

            case Veldrid.PixelFormat.R16_G16_B16_A16_UInt:
                return PixelFormat.R16G16B16A16_UInt;

            case Veldrid.PixelFormat.R16_G16_B16_A16_SInt:
                return PixelFormat.R16G16B16A16_SInt;

            case Veldrid.PixelFormat.R16_G16_B16_A16_Float:
                return PixelFormat.R16G16B16A16_Float;

            case Veldrid.PixelFormat.R32_G32_B32_A32_SInt:
                return PixelFormat.R32G32B32A32_SInt;

            case Veldrid.PixelFormat.ETC2_R8_G8_B8_UNorm:
                return PixelFormat.ETC2_R8G8B8_UNorm;

            case Veldrid.PixelFormat.ETC2_R8_G8_B8_A1_UNorm:
                return PixelFormat.ETC2_R8G8B8A1_UNorm;

            case Veldrid.PixelFormat.ETC2_R8_G8_B8_A8_UNorm:
                return PixelFormat.ETC2_R8G8B8A8_UNorm;

            case Veldrid.PixelFormat.BC4_UNorm:
                return PixelFormat.BC4_UNorm;

            case Veldrid.PixelFormat.BC4_SNorm:
                return PixelFormat.BC4_SNorm;

            case Veldrid.PixelFormat.BC5_UNorm:
                return PixelFormat.BC5_UNorm;

            case Veldrid.PixelFormat.BC5_SNorm:
                return PixelFormat.BC5_SNorm;

            case Veldrid.PixelFormat.BC7_UNorm:
                return PixelFormat.BC7_UNorm;

            case Veldrid.PixelFormat.R8_G8_B8_A8_UNorm_SRgb:
                return PixelFormat.R8G8B8A8_UNorm_SRgb;

            case Veldrid.PixelFormat.B8_G8_R8_A8_UNorm_SRgb:
                return PixelFormat.B8G8R8A8_UNorm_SRgb;

            case Veldrid.PixelFormat.BC1_Rgb_UNorm_SRgb:
                return PixelFormat.BC1_RGB_UNorm;

            case Veldrid.PixelFormat.BC1_Rgba_UNorm_SRgb:
                return PixelFormat.BC1_RGBA_UNorm_SRgb;

            case Veldrid.PixelFormat.BC2_UNorm_SRgb:
                return PixelFormat.BC2_UNorm_SRgb;

            case Veldrid.PixelFormat.BC3_UNorm_SRgb:
                return PixelFormat.BC3_UNorm_SRgb;

            case Veldrid.PixelFormat.BC7_UNorm_SRgb:
                return PixelFormat.BC7_UNorm_SRgb;

            default:
                throw new ArgumentOutOfRangeException(nameof(format));
        }
    }
}
