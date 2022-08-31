// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Framework.Graphics;

 /// <summary>
/// A bitmask indicating how a <see cref="ITexture"/> is permitted to be used.
/// </summary>
[Flags]
public enum TextureUsage : byte
{
    /// <summary>
    /// The Texture can be used as the target of a read-only <see cref="ITexture"/>, and can be accessed from a shader.
    /// </summary>
    Sampled = 1 << 0,

    /// <summary>
    /// The Texture can be used as the target of a read-write <see cref="ITexture"/>, and can be accessed from a shader.
    /// </summary>
    Storage = 1 << 1,

    /// <summary>
    /// The Texture can be used as the color target of a <see cref="IFramebuffer"/>.
    /// </summary>
    RenderTarget = 1 << 2,

    /// <summary>
    /// The Texture can be used as the depth target of a <see cref="IFramebuffer"/>.
    /// </summary>
    DepthStencil = 1 << 3,

    /// <summary>
    /// The Texture is a two-dimensional cubemap.
    /// </summary>
    Cubemap = 1 << 4,

    /// <summary>
    /// The Texture is used as a read-write staging resource for uploading Texture data.
    /// With this flag, a Texture can be mapped using the <see cref="IGraphicsDevice.Map(ITexture, MapMode, uint)"/>
    /// method.
    /// </summary>
    Staging = 1 << 5,

    /// <summary>
    /// The Texture supports automatic generation of mipmaps.
    /// </summary>
    GenerateMipmaps = 1 << 6,

}
