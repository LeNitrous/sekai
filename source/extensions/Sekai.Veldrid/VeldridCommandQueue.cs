// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using Sekai.Framework.Graphics;
using Vd = Veldrid;

namespace Sekai.Veldrid;

internal class VeldridCommandQueue : VeldridGraphicsResource<Vd.CommandList>, ICommandQueue
{
    private VeldridFramebuffer framebuffer = null!;
    private PipelineKind currentPipelineKind;

    public VeldridCommandQueue(Vd.CommandList resource)
        : base(resource)
    {
    }

    public void Begin()
    {
        Resource.Begin();
    }

    public void Clear(uint index, ClearInfo info)
    {
        Resource.ClearColorTarget(index, info.Color.ToVeldrid());

        if (framebuffer.DepthTarget != null)
            Resource.ClearDepthStencil((float)info.Depth, (byte)info.Stencil);
    }

    public void Dispatch(uint x, uint y, uint z)
    {
        Resource.Dispatch(x, y, z);
    }

    public void DispatchIndirect(IBuffer buffer, uint offset)
    {
        Resource.DispatchIndirect(((VeldridBuffer)buffer).Resource, offset);
    }

    public void Draw(uint vertexCount, uint instanceCount, uint vertexStart, uint instanceStart)
    {
        Resource.Draw(vertexCount, instanceCount, vertexStart, instanceStart);
    }

    public void DrawIndexed(uint indexCount, uint instanceCount, uint indexStart, int vertexOffset, uint instanceStart)
    {
        Resource.DrawIndexed(indexCount, instanceCount, indexStart, vertexOffset, instanceStart);
    }

    public void DrawIndexedIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride)
    {
        Resource.DrawIndexedIndirect(((VeldridBuffer)buffer).Resource, offset, drawCount, stride);
    }

    public void DrawIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride)
    {
        Resource.DrawIndirect(((VeldridBuffer)buffer).Resource, offset, drawCount, stride);
    }

    public void End()
    {
        Resource.End();
    }

    public void SetIndexBuffer(IBuffer buffer, IndexBufferFormat format)
    {
        Resource.SetIndexBuffer(((VeldridBuffer)buffer).Resource, format.ToVeldrid());
    }

    public void SetVertexBuffer(uint index, IBuffer buffer, uint offset = 0)
    {
        Resource.SetVertexBuffer(index, ((VeldridBuffer)buffer).Resource, offset);
    }

    public void SetPipeline(IPipeline pipeline)
    {
        currentPipelineKind = pipeline.Kind;
        Resource.SetPipeline(((VeldridPipeline)pipeline).Resource);
    }

    public void SetResourceSet(uint slot, IResourceSet resourceSet)
    {
        switch (currentPipelineKind)
        {
            case PipelineKind.Graphics:
                Resource.SetGraphicsResourceSet(slot, ((VeldridResourceSet)resourceSet).Resource);
                break;

            case PipelineKind.Compute:
                Resource.SetComputeResourceSet(slot, ((VeldridResourceSet)resourceSet).Resource);
                break;
        }
    }

    public void SetResourceSet(uint slot, IResourceSet resourceSet, uint[] dynamicOffsets)
    {
        switch (currentPipelineKind)
        {
            case PipelineKind.Graphics:
                Resource.SetGraphicsResourceSet(slot, ((VeldridResourceSet)resourceSet).Resource, dynamicOffsets);
                break;

            case PipelineKind.Compute:
                Resource.SetComputeResourceSet(slot, ((VeldridResourceSet)resourceSet).Resource, dynamicOffsets);
                break;
        }
    }

    public void SetResourceSet(uint slot, IResourceSet resourceSet, uint dynamicOffsetsCount, ref uint dynamicOffsets)
    {
        switch (currentPipelineKind)
        {
            case PipelineKind.Graphics:
                Resource.SetGraphicsResourceSet(slot, ((VeldridResourceSet)resourceSet).Resource, dynamicOffsetsCount, ref dynamicOffsets);
                break;

            case PipelineKind.Compute:
                Resource.SetComputeResourceSet(slot, ((VeldridResourceSet)resourceSet).Resource, dynamicOffsetsCount, ref dynamicOffsets);
                break;
        }
    }

    public void SetViewport(uint index, Viewport viewport)
    {
        Resource.SetViewport(index, viewport.ToVeldrid());
    }

    public void SetFramebuffer(IFramebuffer framebuffer)
    {
        this.framebuffer = (VeldridFramebuffer)framebuffer;
        Resource.SetFramebuffer(this.framebuffer.Resource);
    }

    public void SetScissor(uint index, Rectangle rect)
    {
        Resource.SetScissorRect(index, (uint)rect.X, (uint)rect.Y, (uint)rect.Width, (uint)rect.Height);
    }
}
