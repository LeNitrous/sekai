// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sekai.Graphics;

/// <summary>
/// Provides access to all graphics-related functionality.
/// </summary>
public abstract class GraphicsDevice : IDisposable
{
    /// <summary>
    /// The backend graphics API used.
    /// </summary>
    public abstract GraphicsAPI API { get; }

    /// <summary>
    /// The device's name.
    /// </summary>
    public abstract string Device { get; }

    /// <summary>
    /// The device's vendor.
    /// </summary>
    public abstract string Vendor { get; }

    /// <summary>
    /// The device's version.
    /// </summary>
    public abstract Version Version { get; }

    /// <summary>
    /// Gets or sets the vertical syncing mode.
    /// </summary>
    public abstract bool SyncMode { get; set; }

    /// <summary>
    /// Makes the calling thread the current to allow graphics calls.
    /// </summary>
    public abstract void MakeCurrent();

    /// <summary>
    /// Clears the back buffer (or framebuffer if set) with the given flags.
    /// </summary>
    /// <param name="info">The clear info to use.</param>
    public abstract void Clear(ClearInfo info);

    /// <summary>
    /// Creates a graphics buffer.
    /// </summary>
    /// <param name="type">The buffer type.</param>
    /// <param name="size">The buffer size.</param>
    /// <param name="dynamic">Whether this buffer will be updated frequently or not.</param>
    /// <returns>A new graphics buffer.</returns>
    public abstract GraphicsBuffer CreateBuffer(BufferType type, uint size, bool dynamic = false);

    /// <inheritdoc cref="CreateBuffer(BufferType, uint, bool)"/>
    /// <param name="data">The data to initially set on the buffer.</param>
    public GraphicsBuffer CreateBuffer(BufferType type, nint data, uint size, bool dynamic = false)
    {
        var buffer = CreateBuffer(type, size, dynamic);
        buffer.SetData(data, size);
        return buffer;
    }

    /// <inheritdoc cref="CreateBuffer(BufferType, uint, bool)"/>
    /// <param name="count">The number of elements the buffer will contain.</param>
    /// <typeparam name="T">The type of content the buffer will contain.</typeparam>
    public GraphicsBuffer CreateBuffer<T>(BufferType type, uint count, bool dynamic = false)
        where T : unmanaged
    {
        return CreateBuffer(type, (uint)(Unsafe.SizeOf<T>() * count), dynamic);
    }

    /// <inheritdoc cref="CreateBuffer(BufferType, uint, bool)"/>
    /// <param name="data">The data to initially set on the buffer.</param>
    /// <typeparam name="T">The type of content the buffer will contain.</typeparam>
    public GraphicsBuffer CreateBuffer<T>(BufferType type, ReadOnlySpan<T> data, bool dynamic = false)
        where T : unmanaged
    {
        var buffer = CreateBuffer<T>(type, (uint)data.Length, dynamic);
        buffer.SetData(data);
        return buffer;
    }

    /// <inheritdoc cref="CreateBuffer{T}(BufferType, ReadOnlySpan{T}, bool)"/>
    public GraphicsBuffer CreateBuffer<T>(BufferType type, T[] data, bool dynamic = false)
        where T : unmanaged
    {
        return CreateBuffer(type, data, dynamic);
    }

    /// <inheritdoc cref="CreateBuffer{T}(BufferType, ReadOnlySpan{T}, bool)"/>
    public GraphicsBuffer CreateBuffer<T>(BufferType type, ref T data, bool dynamic = false)
        where T : unmanaged
    {
        return CreateBuffer(type, MemoryMarshal.CreateReadOnlySpan(ref data, 1), dynamic);
    }

    /// <summary>
    /// Creates a texture.
    /// </summary>
    /// <param name="description">The texture descriptor.</param>
    /// <returns>A new texture.</returns>
    public abstract Texture CreateTexture(TextureDescription description);

    /// <summary>
    /// Creates a shader.
    /// </summary>
    /// <param name="attachments">The shader code to include.</param>
    /// <returns>A new shader.</returns>
    public abstract Shader CreateShader(params ShaderCode[] attachments);

    /// <summary>
    /// Creates a new rasterizer state.
    /// </summary>
    /// <param name="description">The rasterizer state descriptor.</param>
    /// <returns>A new rasterizer state.</returns>
    public abstract RasterizerState CreateRasterizerState(RasterizerStateDescription description);

    /// <summary>
    /// Creates a new blend state.
    /// </summary>
    /// <param name="description">The blend state descriptor.</param>
    /// <returns>A new blend state.</returns>
    public abstract BlendState CreateBlendState(BlendStateDescription description);

    /// <summary>
    /// Creates a new depth stencil state.
    /// </summary>
    /// <param name="description">The depth stencil state descriptor.</param>
    /// <returns>A new depth stencil state.</returns>
    public abstract DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description);

    /// <summary>
    /// Creates a new sampler state.
    /// </summary>
    /// <param name="description">The sampler state descriptor.</param>
    /// <returns>A new sampler state.</returns>
    public abstract Sampler CreateSampler(SamplerDescription description);

    /// <summary>
    /// Creates a new framebuffer.
    /// </summary>
    /// <param name="depth">The depth attachments to include to the framebuffer.</param>
    /// <param name="color">The color attachments to include to the framebuffer.</param>
    /// <returns>A new framebuffer.</returns>
    public abstract Framebuffer CreateFramebuffer(FramebufferAttachment? depth, params FramebufferAttachment[] color);

    /// <summary>
    /// Sets the shader to be used on the next draw call.
    /// </summary>
    /// <param name="shader">The shader to use.</param>
    public abstract void SetShader(Shader shader);

    /// <summary>
    /// Sets the texture to be used on the next draw call.
    /// </summary>
    /// <param name="texture">The texture to use.</param>
    /// <param name="slot">The binding slot that this texture will be used in.</param>
    public abstract void SetTexture(Texture texture, uint slot);

    /// <summary>
    /// Sets the sampler to be used on the next draw call.
    /// </summary>
    /// <param name="sampler">The sampler to use.</param>
    /// <param name="slot">The binding slot that this texture will be used in.</param>
    public abstract void SetSampler(Sampler sampler, uint slot);

    /// <summary>
    /// Sets the rasterizer state to be used on the next draw call.
    /// </summary>
    /// <param name="state">The rasterizer state to use.</param>
    public abstract void SetRasterizerState(RasterizerState state);

    /// <summary>
    /// Sets the blend state to be used on the next draw call.
    /// </summary>
    /// <param name="state">The blend state to use.</param>
    public abstract void SetBlendState(BlendState state);

    /// <summary>
    /// Sets the depth stencil state to be used on the next draw call.
    /// </summary>
    /// <param name="state">The depth stencil state to use.</param>
    /// <param name="reference"/>The stencil reference to use.</param>
    public abstract void SetDepthStencilState(DepthStencilState state, int reference);

    /// <summary>
    /// Sets the viewport of the device.
    /// </summary>
    public abstract void SetViewport(Rectangle viewport);

    /// <summary>
    /// Sets the scissor rectangle of the device.
    /// </summary>
    public abstract void SetScissor(Rectangle scissor);

    /// <summary>
    /// Sets the vertex buffer to be used on the next draw call.
    /// </summary>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="layout">The input layout that this vertex buffer will use.</param>
    public abstract void SetVertexBuffer(GraphicsBuffer buffer, VertexLayout layout);

    /// <summary>
    /// Sets the index buffer to be used on the next draw call.
    /// </summary>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="type">The type of indices.</param>
    public abstract void SetIndexBuffer(GraphicsBuffer buffer, IndexType type);

    /// <summary>
    /// Sets the uniform buffer to be used on the next draw call.
    /// </summary>
    /// <param name="buffer">The buffer to use.</param>
    /// <param name="slot">The buffer slot.</param>
    public abstract void SetUniformBuffer(GraphicsBuffer buffer, uint slot);

    /// <summary>
    /// Sets the framebuffer to be used on the next draw call.
    /// </summary>
    /// <param name="framebuffer">The framebuffer to use.</param>
    /// <remarks>
    /// Passing a <see langword="null"/> value uses the default back buffer instead.
    /// </remarks>
    public abstract void SetFramebuffer(Framebuffer? framebuffer);

    /// <summary>
    /// Draws using the current vertex buffer.
    /// </summary>
    /// <param name="type">The primitive type to use in drawing.</param>
    /// <param name="vertexCount">The number of vertices to draw.</param>
    /// <param name="vertexStart">The starting offset from the set vertex buffer.</param>
    /// <param name="instanceCount">The number of instances to draw.</param>
    /// <param name="instanceStart">The starting offset for fetching vertex attributes.</param>
    public abstract void Draw(PrimitiveType type, uint vertexCount, uint vertexStart, uint instanceCount, uint instanceStart);

    /// <summary>
    /// Draws using the current vertex and index buffer.
    /// </summary>
    /// <param name="type">The primitive type to use in drawing.</param>
    /// <param name="indexCount">The number of indices to draw.</param>
    /// <param name="indexStart">The starting offset from the set index buffer.</param>
    /// <param name="vertexStart">The starting offset from the set vertex buffer.</param>
    /// <param name="instanceCount">The number of instances to draw.</param>
    /// <param name="instanceStart">The starting offset for fetching vertex attributes.</param>
    public abstract void DrawIndexed(PrimitiveType type, uint indexCount, uint indexStart, uint vertexStart, uint instanceCount, uint instanceStart);

    /// <summary>
    /// Draws using the current vertex and indirect buffer.
    /// </summary>
    /// <param name="buffer">The indirect buffer to use.</param>
    /// <param name="type">The primitive type to use in drawing.</param>
    /// <param name="offset">The starting offset from the indirect buffer.</param>
    /// <param name="drawCount">The number of times to draw.</param>
    /// <param name="stride">The indirect buffer's stride.</param>
    public abstract void DrawIndirect(GraphicsBuffer buffer, PrimitiveType type, uint offset, uint drawCount, uint stride);

    /// <summary>
    /// Draws using the current vertex, index and indirect buffer.
    /// </summary>
    /// <param name="buffer">The indirect buffer to use.</param>
    /// <param name="type">The primitive type to use in drawing.</param>
    /// <param name="offset">The starting offset from the indirect buffer.</param>
    /// <param name="drawCount">The number of times to draw.</param>
    /// <param name="stride">The indirect buffer's stride.</param>
    public abstract void DrawIndexedIndirect(GraphicsBuffer buffer, PrimitiveType type, uint offset, uint drawCount, uint stride);

    /// <inheritdoc cref="Draw(PrimitiveType, uint, uint, uint, uint)"/>
    public void Draw(PrimitiveType type, uint vertexCount)
    {
        Draw(type, vertexCount, 0);
    }

    /// <inheritdoc cref="Draw(PrimitiveType, uint, uint, uint, uint)"/>
    public void Draw(PrimitiveType type, uint vertexCount, uint vertexStart)
    {
        Draw(type, vertexCount,vertexStart, 1, 0);
    }

    /// <inheritdoc cref="DrawIndexed(PrimitiveType, uint, uint, uint, uint, uint)"/>
    public void DrawIndexed(PrimitiveType type, uint indexCount)
    {
        DrawIndexed(type, indexCount, 0);
    }

    /// <inheritdoc cref="DrawIndexed(PrimitiveType, uint, uint, uint, uint, uint)"/>
    public void DrawIndexed(PrimitiveType type, uint indexCount, uint indexStart)
    {
        DrawIndexed(type, indexCount, indexStart, 0);
    }

    /// <inheritdoc cref="DrawIndexed(PrimitiveType, uint, uint, uint, uint, uint)"/>
    public void DrawIndexed(PrimitiveType type, uint indexCount, uint indexStart, uint vertexStart)
    {
        DrawIndexed(type, indexCount, indexStart, vertexStart, 1, 0);
    }

    /// <summary>
    /// Present the back buffer.
    /// </summary>
    public abstract void Present();

    /// <summary>
    /// Dispatch the current compute shader.
    /// </summary>
    /// <param name="groupCountX">The number of thread groups in X.</param>
    /// <param name="groupCountY">The number of thread groups in Y.</param>
    /// <param name="groupCountZ">The number of thread groups in Z.</param>
    public abstract void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ);

    /// <summary>
    /// Dispatch the current compute shader using an indirect buffer.
    /// </summary>
    /// <param name="buffer">The indirect buffer to use.</param>
    /// <param name="offset">The offset to start reading data from.</param>
    public abstract void DispatchIndirect(GraphicsBuffer buffer, uint offset);

    /// <summary>
    /// Force the device to execute all queued commands in the command buffer.
    /// </summary>
    public abstract void Flush();

    public abstract void Dispose();
}
