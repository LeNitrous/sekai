// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// An enumeration of features in a graphics device.
/// </summary>
[Flags]
public enum GraphicsDeviceFeatures : long
{
    /// <summary>
    /// No features.
    /// </summary>
    None = 0,

    /// <summary>
    /// Compute shaders.
    /// </summary>
    ShaderCompute = 1,

    /// <summary>
    /// Geometry shaders.
    /// </summary>
    ShaderGeometry = 2,

    /// <summary>
    /// Tesselation shaders.
    /// </summary>
    ShaderTesselation = 4,

    /// <summary>
    /// 64-bit floating point integers in shaders.
    /// </summary>
    ShaderFloat64 = 8,

    /// <summary>
    /// Multiple viewports.
    /// </summary>
    MultipleViewports = 16,

    /// <summary>
    /// Non-zero <see cref="Sampler.LODBias"/>.
    /// </summary>
    SamplerLODBias = 32,

    /// <summary>
    /// <see cref="TextureFilter.Anisotropic"/>.
    /// </summary>
    SamplerAnisotropy = 64,

    /// <summary>
    /// Non-zero base vertex drawing.
    /// </summary>
    DrawBaseVertex = 128,

    /// <summary>
    /// Non-zero base instance drawing.
    /// </summary>
    DrawBaseInstance = 256,

    /// <summary>
    /// Indirect drawing.
    /// </summary>
    DrawIndirect = 512,

    /// <summary>
    /// Indirect non-zero base instance drawing.
    /// </summary>
    DrawIndirectBaseInstance = 1024,

    /// <summary>
    /// <see cref="FillMode.Wireframe"/>.
    /// </summary>
    FillModeWireframe = 2048,

    /// <summary>
    /// <see cref="TextureType.Texture1D"/>.
    /// </summary>
    Texture1D = 4096,

    /// <summary>
    /// Subset texture views.
    /// </summary>
    TextureSubsetView = 8192,

    /// <summary>
    /// Disabled depth clipping.
    /// </summary>
    DepthClipDisable = 16384,

    /// <summary>
    /// 0 to 1 depth range.
    /// </summary>
    DepthRangeZeroToOne = 32768,

    /// <summary>
    /// Inverted Y clip space.
    /// </summary>
    ClipSpaceYInverted = 65536,

    /// <summary>
    /// UV origin is top left.
    /// </summary>
    UVOriginTopLeft = 131072,

    /// <summary>
    /// Per-framebuffer attachment blending.
    /// </summary>
    FramebufferAttachmentBlending = 524288,

    /// <summary>
    /// Stuctured buffers.
    /// </summary>
    BufferStructured = 1048576,

    /// <summary>
    /// Subset buffer views.
    /// </summary>
    BufferSubsetView = 2097152,

    /// <summary>
    /// <see cref="SyncMode.Adaptive"/>.
    /// </summary>
    AdaptiveSyncMode = 4194304,
}
