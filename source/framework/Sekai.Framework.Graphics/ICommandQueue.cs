// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Framework.Graphics;

public interface ICommandQueue : IGraphicsResource
{
    void Begin();
    void End();
    void SetPipeline(IPipeline pipeline);
    void SetIndexBuffer(IBuffer buffer, IndexBufferFormat kind);
    void SetVertexBuffer(uint index, IBuffer buffer, uint offset = 0);
    void SetResourceSet(uint slot, IResourceSet resourceSet);
    void SetResourceSet(uint slot, IResourceSet resourceSet, uint[] dynamicOffsets);
    void SetResourceSet(uint slot, IResourceSet resourceSet, uint dynamicOffsetsCount, ref uint dynamicOffsets);
    void SetViewport(uint index, Viewport viewport);
    void Clear(uint index, ClearInfo info);
    void Draw(uint vertexCount, uint instanceCount, uint vertexStart, uint instanceStart);
    void DrawIndexed(uint indexCount, uint instanceCount, uint indexStart, uint vertexStart, int vertexOffset, uint instanceStart);
    void DrawIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride);
    void DrawIndexedIndirect(IBuffer buffer, uint offset, uint drawCount, uint stride);
    void Dispatch(uint x, uint y, uint z);
    void DispatchIndirect(IBuffer buffer, uint offset);
}
