// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;

namespace Sekai.Framework.Graphics;

public interface ICommandQueue : IGraphicsResource
{
    /// <summary>
    /// Starts execution of commands.
    /// </summary>
    void Begin();

    /// <summary>
    /// Ends execution of commands.
    /// </summary>
    void End();

    /// <summary>
    /// Sets the current pipeline.
    /// </summary>
    void SetPipeline(IPipeline pipeline);

    /// <summary>
    /// Sets the current frame buffer.
    /// </summary>
    void SetFramebuffer(IFramebuffer framebuffer);

    /// <summary>
    /// Sets the current index buffer.
    /// </summary>
    void SetIndexBuffer(IBuffer buffer, IndexBufferFormat kind);

    /// <summary>
    /// Sets the current vertex buffer.
    /// </summary>
    void SetVertexBuffer(uint index, IBuffer buffer, uint offset = 0);

    /// <summary>
    /// Sets the current resource set.
    /// </summary>
    void SetResourceSet(uint slot, IResourceSet resourceSet);

    /// <summary>
    /// Sets the current resource set.
    /// </summary>
    void SetResourceSet(uint slot, IResourceSet resourceSet, uint[] dynamicOffsets);

    /// <summary>
    /// Sets the current resource set.
    /// </summary>
    void SetResourceSet(uint slot, IResourceSet resourceSet, uint dynamicOffsetsCount, ref uint dynamicOffsets);

    /// <summary>
    /// Sets the scissor rectangle.
    /// </summary>
    void SetScissor(uint index, Rectangle rect);


    /// <summary>
    /// Sets the viewport.
    /// </summary>
    void SetViewport(uint index, Viewport viewport);

    /// <summary>
    /// Clears the current frame buffer's color and depth targets.
    /// </summary>
    void Clear(uint index, ClearInfo info);

    /// <summary>
    /// Draws vertices to the current frame buffer.
    /// </summary>
    void Draw(uint vertexCount, uint instanceCount, uint vertexStart, uint instanceStart);

    /// <summary>
    /// Draws vertices to the current frame buffer.
    /// </summary>
    void DrawIndexed(uint indexCount, uint instanceCount, uint indexStart, int vertexOffset, uint instanceStart);

    /// <summary>
    /// Draws vertices to the current frame buffer using an indirect buffer.
    /// </summary>
    void DrawIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride);

    /// <summary>
    /// Draws vertices to the current frame buffer using an indirect buffer.
    /// </summary>
    void DrawIndexedIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride);

    /// <summary>
    /// Dispatches the current compute shader.
    /// </summary>
    void Dispatch(uint x, uint y, uint z);

    /// <summary>
    /// Dispatches the current compute shader using an indirect buffer.
    /// </summary>
    void DispatchIndirect(IBuffer buffer, uint offset);
}

public static class CommandQueueExtensions
{
    public static void SetVertexBuffer(this ICommandQueue queue, VertexBuffer buffer)
    {
        buffer.Bind(queue);
    }

    public static void SetIndexBuffer(this ICommandQueue queue, IndexBuffer buffer)
    {
        buffer.Bind(queue);
    }
}
