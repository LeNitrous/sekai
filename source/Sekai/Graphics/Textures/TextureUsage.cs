// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Graphics.Textures;

/// <summary>
/// The nature of usage of a <see cref="Texture"/>
/// </summary>
public enum TextureUsage
{
    /// <summary>
    /// The textured is sampled through a shader.
    /// </summary>
    Sampled,

    /// <summary>
    /// The texture can be used as a color taget of a <see cref="FrameBuffer"/>.
    /// </summary>
    RenderTarget,

    /// <summary>
    /// The texture can be used as a depth target of a <see cref="Framebuffer"/>.
    /// </summary>
    DepthStencil,

    /// <summary>
    /// THe texture is a cubemap texture.
    /// </summary>
    Cubemap,
}
