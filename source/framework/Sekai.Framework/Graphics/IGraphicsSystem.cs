// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework.Graphics.Buffers;
using Sekai.Framework.Graphics.Shaders;
using Sekai.Framework.Graphics.Textures;
using Sekai.Framework.Graphics.Vertices;
using Sekai.Framework.Windowing;

namespace Sekai.Framework.Graphics;

public interface IGraphicsSystem : IDisposable
{
    /// <summary>
    /// Initializes the window to use this graphics system.
    /// </summary>
    void Initialize(IView view);

    /// <summary>
    /// Presents the current frame to the window.
    /// </summary>
    void Present();

    /// <summary>
    /// Resets the current state.
    /// </summary>
    void Reset();

    /// <summary>
    /// Clears the current framebuffer.
    /// </summary>
    void Clear(ClearInfo clear);

    /// <summary>
    /// Sets the depth.
    /// </summary>
    void SetDepth(DepthInfo depth);

    /// <summary>
    /// Sets the stencil.
    /// </summary>
    void SetStencil(StencilInfo stencil);

    /// <summary>
    /// Sets the viewport rectangle.
    /// </summary>
    void SetViewport(Rectangle viewport);

    /// <summary>
    /// Sets the scissor rectangle.
    /// </summary>
    void SetScissor(Rectangle scissor);

    /// <summary>
    /// Sets the scissor state.
    /// </summary>
    void SetScissorState(bool enabled);

    /// <summary>
    /// Sets the blending.
    /// </summary>
    void SetBlend(BlendingParameters blending);

    /// <summary>
    /// Sets the blending mask.
    /// </summary>
    void SetBlendMask(BlendingMask mask);

    /// <summary>
    /// Sets the texture at a given unit.
    /// </summary>
    void SetTexture(INativeTexture? texture, int unit);

    /// <summary>
    /// Sets the current active shader.
    /// </summary>
    void SetShader(INativeShader shader);

    /// <summary>
    /// Sets the current index buffer.
    /// </summary>
    void SetIndexBuffer(INativeBuffer buffer, IndexFormat format);

    /// <summary>
    /// Sets the current vertex buffer.
    /// </summary>
    void SetVertexBuffer(INativeBuffer buffer, IVertexLayout layout);

    /// <summary>
    /// Draws the currently bound vertex and index buffer using the currently bound shader to the currently bound framebuffer.
    /// </summary>
    void Draw(int count, PrimitiveTopology topology);

    /// <summary>
    /// Creates a graphics resource factory for this graphics system.
    /// </summary>
    IGraphicsFactory CreateFactory();
}
