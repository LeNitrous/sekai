// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;

namespace Sekai.Graphics;

public abstract class GraphicsSystem : DependencyObject
{
    /// <summary>
    /// Clears the current framebuffer.
    /// </summary>
    public abstract void Clear(ClearInfo clear);

    /// <summary>
    /// Makes the graphics system available on the calling thread.
    /// </summary>
    public abstract void MakeCurrent();

    /// <summary>
    /// Creates a shader transpiler for this graphics system.
    /// </summary>
    public abstract ShaderTranspiler CreateShaderTranspiler();

    /// <summary>
    /// Creates a shader.
    /// </summary>
    public abstract NativeShader CreateShader(ShaderTranspileResult result);

    /// <summary>
    /// Creates a buffer.
    /// </summary>
    public abstract NativeBuffer CreateBuffer(int capacity, bool dynamic);

    /// <summary>
    /// Creates a texture.
    /// </summary>
    public abstract NativeTexture CreateTexture(int width, int height, int depth, int level, int layers, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, TextureType type, TextureUsage usage, TextureSampleCount sampleCount, PixelFormat format);

    /// <summary>
    /// Creates a render target.
    /// </summary>
    public abstract NativeRenderTarget CreateRenderTarget(IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth = null);

    /// <summary>
    /// Draws the currently bound vertex buffer using the currently bound shader to the currently bound render target.
    /// </summary>
    public abstract void Draw(uint vertexCount, PrimitiveTopology topology, uint instanceCount, int baseVertex, uint baseInstance);

    /// <summary>
    /// Draws the currently bound vertex and index buffer using the currently bound shader to the currently bound render target.
    /// </summary>
    public abstract void DrawIndexed(uint indexCount, PrimitiveTopology topology, uint instanceCount, uint indexOffset, int baseVertex, uint baseInstance);

    /// <summary>
    /// Draws the currently bound vertex and index buffer using the currently bound shader to the currently bound render target using an indirect buffer.
    /// </summary>
    public abstract void DrawIndexedIndirect(NativeBuffer buffer, PrimitiveTopology topology, uint offset, uint drawCount);

    /// <summary>
    /// Draws the currently bound vertex buffer using the currently bound shader to the currently bound render target using an indirect buffer.
    /// </summary>
    public abstract void DrawIndirect(NativeBuffer buffer, PrimitiveTopology topology, uint offset, uint drawCount);

    /// <summary>
    /// Presents the current frame to the window.
    /// </summary>
    public abstract void Present();

    /// <summary>
    /// Called at the start of the rendering step for preparing graphics state.
    /// </summary>
    public abstract void Prepare();

    /// <summary>
    /// Sets the blending state.
    /// </summary>
    public abstract void SetBlend(BlendInfo blending);

    /// <summary>
    /// Sets the current index buffer.
    /// </summary>
    public abstract void SetBuffer(NativeBuffer? buffer = null, IndexFormat format = IndexFormat.UInt16);

    /// <summary>
    /// Sets the current vertex buffer.
    /// </summary>
    public abstract void SetBuffer(NativeBuffer? buffer = null, IVertexLayout? layout = null);

    /// <summary>
    /// Sets the depth.
    /// </summary>
    public abstract void SetDepth(DepthInfo depth);

    /// <summary>
    /// Sets the face culling.
    /// </summary>
    public abstract void SetFaceCulling(FaceCulling culling);

    /// <summary>
    /// Sets the face winding.
    /// </summary>
    public abstract void SetFaceWinding(FaceWinding winding);

    /// <summary>
    /// Sets the current framebuffer.
    /// </summary>
    public abstract void SetRenderTarget(NativeRenderTarget? buffer = null);

    /// <summary>
    /// Sets the scissor state.
    /// </summary>
    public abstract void SetScissor(ScissorInfo scissor);

    /// <summary>
    /// Sets the current shader.
    /// </summary>
    public abstract void SetShader(NativeShader? shader = null);

    /// <summary>
    /// Sets the stencil.
    /// </summary>
    public abstract void SetStencil(StencilInfo stencil);

    /// <summary>
    /// Sets the texture at a given unit.
    /// </summary>
    public abstract void SetTexture(NativeTexture? texture = null, int unit = 0);

    /// <summary>
    /// Sets the viewport.
    /// </summary>
    public abstract void SetViewport(Viewport viewport);
}
