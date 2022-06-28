// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics.Buffers;

/// <summary>
/// Determines how the texture will be used in a <see cref="Framebuffer"/>.
/// </summary>
public enum FramebufferAttachmentKind
{
    /// <summary>
    /// The texture will be used as a render target.
    /// </summary>
    /// <remarks>
    /// The texture must have <see cref="Textures.TextureUsage.RenderTarget"/>.
    /// </remarks>
    ColorTarget,

    /// <summary>
    /// The texture will be used as a depth stencil.
    /// </summary>
    /// <remarks>
    /// The texture must have <see cref="Textures.TextureUsage.DepthStencil"/>.
    /// </remarks>
    DepthTarget,
}
