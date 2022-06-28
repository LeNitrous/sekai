// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics.Textures;

/// <summary>
/// Flags indicating how the <see cref="Texture"/> will be used.
/// </summary>
[Flags]
public enum TextureUsage : byte
{
    /// <summary>
    /// The texture is usable as a readable shader resource.
    /// </summary>
    Sampled = 0x1,

    /// <summary>
    /// The texture is usable as a writable shader resource.
    /// </summary>
    Storage = 0x2,

    /// <summary>
    /// The texture is usable as a framebuffer color target.
    /// </summary>
    RenderTarget = 0x4,

    /// <summary>
    /// The texture is usable as a framebuffer depth target.
    /// </summary>
    DepthStencil = 0x8,

    /// <summary>
    /// The texture is a two-dimensional cubemap.
    /// </summary>
    Cubemap = 0x10,

    /// <summary>
    /// The texture can be used to transfer data between the CPU and GPU.
    /// </summary>
    Staging = 0x20,

    /// <summary>
    /// The texture supports automatic generation of mipmaps.
    /// </summary>
    GenerateMipmaps = 0x40,
}
