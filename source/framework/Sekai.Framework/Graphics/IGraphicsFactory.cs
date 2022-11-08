// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Graphics.Buffers;
using Sekai.Framework.Graphics.Shaders;
using Sekai.Framework.Graphics.Textures;

namespace Sekai.Framework.Graphics;

/// <summary>
/// Factory for creating graphics resources.
/// </summary>
public interface IGraphicsFactory
{
    /// <summary>
    /// Creates a shader used for computing.
    /// </summary>
    INativeShader CreateShader(string comp);

    /// <summary>
    /// Creates a shader used for graphics.
    /// </summary>
    INativeShader CreateShader(string vert, string frag);

    /// <summary>
    /// Creates a buffer.
    /// </summary>
    INativeBuffer CreateBuffer(int capacity, bool dynamic);

    /// <summary>
    /// Creates a texture.
    /// </summary>
    INativeTexture CreateTexture();

    /// <summary>
    /// Creates a framebuffer.
    /// </summary>
    INativeFrameBuffer CreateFramebuffer(INativeTexture color, RenderBufferFormat[]? depth);
}
