// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics;

/// <summary>
/// Flags to describe how a texture is used.
/// </summary>
[Flags]
public enum TextureUsage
{
    /// <summary>
    /// This texture is unused.
    /// </summary>
    None,

    /// <summary>
    /// This texture is used as a shader resource.
    /// </summary>
    Resource,

    /// <summary>
    /// This texture is used as a framebuffer render target.
    /// </summary>
    RenderTarget
}
