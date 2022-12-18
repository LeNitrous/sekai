// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics.Textures;

namespace Sekai.Graphics.Buffers;

/// <summary>
/// A framebuffer graphics device.
/// </summary>
public interface INativeFrameBuffer : IDisposable
{
    /// <summary>
    /// Sets a framebuffer depth attachment.
    /// </summary>
    /// <param name="texture">The texture to attach.</param>
    /// <param name="level">The texture's level to attach.</param>
    /// <param name="layer">The texture's layer to attach.</param>
    void SetDepthAttachment(INativeTexture texture, int level, int layer);

    /// <summary>
    /// Adds a framebuffer color attachment.
    /// </summary>
    /// <param name="texture">The texture to attach.</param>
    /// <param name="level">The texture's level to attach.</param>
    /// <param name="layer">The texture's layer to attach.</param>
    void AddColorAttachment(INativeTexture texture, int level, int layer);
}
