// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Drawing;
using Sekai.Framework;
using Sekai.Framework.Graphics;

namespace Sekai.Dummy;

internal class DummyCommandQueue : FrameworkObject, ICommandQueue
{
    public void Begin()
    {
    }

    public void Clear(uint index, ClearInfo info)
    {
    }

    public void Dispatch(uint x, uint y, uint z)
    {
    }

    public void DispatchIndirect(IBuffer buffer, uint offset)
    {
    }

    public void Draw(uint vertexCount, uint instanceCount, uint vertexStart, uint instanceStart)
    {
    }

    public void DrawIndexed(uint indexCount, uint instanceCount, uint indexStart, int vertexOffset, uint instanceStart)
    {
    }

    public void DrawIndexedIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride)
    {
    }

    public void DrawIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride)
    {
    }

    public void End()
    {
    }

    public void SetFramebuffer(IFramebuffer framebuffer)
    {
    }

    public void SetIndexBuffer(IBuffer buffer, IndexBufferFormat kind)
    {
    }

    public void SetPipeline(IPipeline pipeline)
    {
    }

    public void SetResourceSet(uint slot, IResourceSet resourceSet)
    {
    }

    public void SetResourceSet(uint slot, IResourceSet resourceSet, uint[] dynamicOffsets)
    {
    }

    public void SetResourceSet(uint slot, IResourceSet resourceSet, uint dynamicOffsetsCount, ref uint dynamicOffsets)
    {
    }

    public void SetScissor(uint index, Rectangle rect)
    {
    }

    public void SetVertexBuffer(uint index, IBuffer buffer, uint offset = 0)
    {
    }

    public void SetViewport(uint index, Viewport viewport)
    {
    }
}
