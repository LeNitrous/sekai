// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework.Extensions;
using Sekai.Framework.Graphics.Buffers;
using Sekai.Framework.Graphics.Vertices;

namespace Sekai.Framework.Graphics;

public sealed partial class CommandList : GraphicsObject<Veldrid.CommandList>
{
    private Framebuffer? framebuffer;
    private IPipeline? pipelineState;
    private int indexCount;
    private int vertexCount;

    /// <summary>
    /// Clears the current frame buffer's color and depth buffers if the frame buffer has a depth buffer.
    /// </summary>
    public void Clear(ClearInfo info)
    {
        ensureValidState();

        if (framebuffer != null)
        {
            if (info.Index > framebuffer.ColorTargets.Count)
                throw new InvalidOperationException();

            Resource.ClearColorTarget((uint)info.Index, info.Color.ToVeldrid());

            if (framebuffer.DepthTarget != null)
                Resource.ClearDepthStencil((float)info.Depth, info.Stencil);
        }

        Resource.ClearColorTarget(0, info.Color.ToVeldrid());
    }

    /// <summary>
    /// Sets the current pipeline for this command list.
    /// </summary>
    public void SetPipeline(IPipeline pipelineState)
    {
        ensureValidState();
        this.pipelineState = pipelineState;
    }

    public void SetVertexBuffer<T>(Buffer<T> vertexBuffer, int slot = 0)
        where T : unmanaged, IVertex
    {
        ensureValidState();
        vertexCount = vertexBuffer.Count;
        Resource.SetVertexBuffer((uint)slot, vertexBuffer.Resource);
    }

    public void SetIndexBuffer<T>(Buffer<T> indexBuffer)
        where T : unmanaged
    {
        ensureValidState();

        if (!index_format_map.TryGetValue(typeof(T), out var format))
            throw new InvalidOperationException($"{nameof(T)} must be a valid index type.");

        indexCount = indexBuffer.Count;
        Resource.SetIndexBuffer(indexBuffer.Resource, format);
    }

    /// <summary>
    /// Sets the current frame buffer.
    /// </summary>
    public void SetFramebuffer(Framebuffer framebuffer)
    {
        ensureValidState();
        this.framebuffer = framebuffer;
        Resource.SetFramebuffer(framebuffer.Resource);
    }

    /// <summary>
    /// Sets the viewport at a given framebuffer color attachment index.
    /// </summary>
    public void SetViewport(Viewport viewport, int index = 0)
    {
        ensureValidState();

        if (framebuffer == null)
            throw new InvalidOperationException("There is no current frame buffer being used.");

        if (index > framebuffer.ColorTargets.Count || index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be greater than or equal to zero or less than the number of the current frame buffer's color targets.");

        Resource.SetViewport((uint)index, viewport.ToVeldrid());
    }

    /// <summary>
    /// Sets the scissor rectangle at a framebuffer given color attachment index.
    /// </summary>
    public void SetScissor(Rectangle rect, int index = 0)
    {
        ensureValidState();

        if (framebuffer == null)
            throw new InvalidOperationException("There is no current frame buffer being used.");

        if (index > framebuffer.ColorTargets.Count || index < 0)
            throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} must be greater than or equal to zero or less than the number of the current frame buffer's color targets.");

        Resource.SetScissorRect((uint)index, (uint)rect.Left, (uint)rect.Top, (uint)rect.Width, (uint)rect.Height);
    }

    /// <summary>
    /// Draws primitives.
    /// </summary>
    public void Draw(int instanceCount = 1, int indexStart = 0, int vertexStart = 0, int instanceStart = 0)
    {
        ensureValidState();

        if (pipelineState is not GraphicsPipelineState graphicsPipelineState)
            throw new InvalidOperationException($"The current pipeline is not a {nameof(GraphicsPipelineState)}.");

        Resource.SetPipeline(Context.FetchPipeline(graphicsPipelineState));

        if (graphicsPipelineState.Shader.ResourceSets != null)
        {
            for (uint i = 0; i < graphicsPipelineState.Shader.ResourceLayouts.Length; i++)
                Resource.SetGraphicsResourceSet(i, graphicsPipelineState.Shader.ResourceSets[i]);
        }

        if (indexCount > 0)
        {
            Resource.DrawIndexed((uint)indexCount, (uint)instanceCount, (uint)indexStart, vertexStart, (uint)instanceStart);
        }
        else
        {
            Resource.Draw((uint)vertexCount, (uint)instanceCount, (uint)vertexStart, (uint)instanceStart);
        }
    }
}
