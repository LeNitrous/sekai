// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

/// <summary>
/// An enumeration of pixel formats.
/// </summary>
public enum PixelFormat
{
    /// <summary>
    /// 1-component normalized unsigned byte.
    /// </summary>
    R8_UNorm,

    /// <summary>
    /// 1-component normalized signed byte.
    /// </summary>
    R8_SNorm,

    /// <summary>
    /// 1-component unsigned byte.
    /// </summary>
    R8_UInt,

    /// <summary>
    /// 1-component signed byte.
    /// </summary>
    R8_SInt,

    /// <summary>
    /// 2-component normalized unsigned byte.
    /// </summary>
    R8G8_UNorm,

    /// <summary>
    /// 2-component normalized signed byte.
    /// </summary>
    R8G8_SNorm,

    /// <summary>
    /// 2-component unsigned byte.
    /// </summary>
    R8G8_UInt,

    /// <summary>
    /// 2-component signed byte.
    /// </summary>
    R8G8_SInt,

    /// <summary>
    /// 4-component normalized unsigned byte.
    /// </summary>
    R8G8B8A8_UNorm,

    /// <summary>
    /// 4-component normalized unsigned byte in sRGB color-space.
    /// </summary>
    R8G8B8A8_UNorm_SRgb,

    /// <summary>
    /// 4-component normalized signed byte.
    /// </summary>
    R8G8B8A8_SNorm,

    /// <summary>
    /// 4-component unsigned byte.
    /// </summary>
    R8G8B8A8_UInt,

    /// <summary>
    /// 4-component signed byte.
    /// </summary>
    R8G8B8A8_SInt,

    /// <summary>
    /// 4-component normalized unsigned byte in BGRA format.
    /// </summary>
    B8G8R8A8_UNorm,

    /// <summary>
    /// 4-component normalized unsigned byte in BGRA format and sRGB color-space.
    /// </summary>
    B8G8R8A8_UNorm_SRgb,

    /// <summary>
    /// 1-component normalized unsigned short.
    /// </summary>
    R16_UNorm,

    /// <summary>
    /// 1-component normalized signed short.
    /// </summary>
    R16_SNorm,

    /// <summary>
    /// 1-component unsigned short.
    /// </summary>
    R16_UInt,

    /// <summary>
    /// 1-component signed short.
    /// </summary>
    R16_SInt,

    /// <summary>
    /// 1-component half-float.
    /// </summary>
    R16_Float,

    /// <summary>
    /// 2-component normalized unsigned short.
    /// </summary>
    R16G16_UNorm,

    /// <summary>
    /// 2-component normalized signed short.
    /// </summary>
    R16G16_SNorm,

    /// <summary>
    /// 2-component unsigned short.
    /// </summary>
    R16G16_UInt,

    /// <summary>
    /// 2-component signed short.
    /// </summary>
    R16G16_SInt,

    /// <summary>
    /// 2-component half-float.
    /// </summary>
    R16G16_Float,

    /// <summary>
    /// 4-component normalized unsigned short.
    /// </summary>
    R16G16B16A16_UNorm,

    /// <summary>
    /// 4-component normalized signed short.
    /// </summary>
    R16G16B16A16_SNorm,

    /// <summary>
    /// 4-component unsigned short.
    /// </summary>
    R16G16B16A16_UInt,

    /// <summary>
    /// 4-component signed short.
    /// </summary>
    R16G16B16A16_SInt,

    /// <summary>
    /// 4-component half-float.
    /// </summary>
    R16G16B16A16_Float,

    /// <summary>
    /// 1-component unsigned 32-bit integer.
    /// </summary>
    R32_UInt,

    /// <summary>
    /// 1-component signed 32-bit integer.
    /// </summary>
    R32_SInt,

    /// <summary>
    /// 1-component float.
    /// </summary>
    R32_Float,

    /// <summary>
    /// 2-component unsigned 32-bit integer.
    /// </summary>
    R32G32_UInt,

    /// <summary>
    /// 2-component signed 32-bit integer.
    /// </summary>
    R32G32_SInt,

    /// <summary>
    /// 2-component float.
    /// </summary>
    R32G32_Float,

    /// <summary>
    /// 4-component unsigned 32-bit integer.
    /// </summary>
    R32G32B32A32_UInt,

    /// <summary>
    /// 4-component signed 32-bit integer.
    /// </summary>
    R32G32B32A32_SInt,

    /// <summary>
    /// 4-component float.
    /// </summary>
    R32G32B32A32_Float,

    /// <summary>
    /// 24-bit depth as unsigned normalized values and 8-bit stencil as unsigned values.
    /// </summary>
    D24_UNorm_S8_UInt,

    /// <summary>
    /// 32-bit depth as float values and 8-bit stencil as unsigned values.
    /// </summary>
    D32_Float_S8_UInt,

    /// <summary>
    /// BC1 (DXT1) compression.
    /// </summary>
    BC1_Rgb_UNorm,

    /// <summary>
    /// BC1 (DXT1) compression with alpha.
    /// </summary>
    BC1_Rgba_UNorm,

    /// <summary>
    /// BC1 (DXT1) compression in sRGB color-space.
    /// </summary>
    BC1_Rgb_UNorm_SRgb,

    /// <summary>
    /// BC1 (DXT1) compression with alpha in sRGB color-space.
    /// </summary>
    BC1_Rgba_UNorm_SRgb,

    /// <summary>
    /// BC2 (DXT3) compression.
    /// </summary>
    BC2_UNorm,

    /// <summary>
    /// BC2 (DXT3) compression in sRGB color-space.
    /// </summary>
    BC2_UNorm_SRgb,

    /// <summary>
    /// BC3 (DXT5) compression.
    /// </summary>
    BC3_UNorm,

    /// <summary>
    /// BC3 (DXT5) compression in sRGB color-space.
    /// </summary>
    BC3_UNorm_SRgb,

    /// <summary>
    /// BC4 compression as unsigned normalized values.
    /// </summary>
    BC4_UNorm,

    /// <summary>
    /// BC4 compression as signed normalized values.
    /// </summary>
    BC4_SNorm,

    /// <summary>
    /// BC5 compression as unsigned normalized values.
    /// </summary>
    BC5_UNorm,

    /// <summary>
    /// BC5 compression as signed normalized values.
    /// </summary>
    BC5_SNorm,

    /// <summary>
    /// BC7 compresison as unsigned normalized values.
    /// </summary>
    BC7_UNorm,

    /// <summary>
    /// BC7 compression as unsigned noramlized values in sRGB color-space.
    /// </summary>
    BC7_UNorm_SRgb,
}
