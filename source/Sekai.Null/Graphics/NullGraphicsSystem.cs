// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Graphics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;

namespace Sekai.Null.Graphics;

internal class NullGraphicsSystem : GraphicsSystem
{
    public override void Clear(ClearInfo clear)
    {
    }

    public override NativeBuffer CreateBuffer(int capacity, bool dynamic)
        => new NullBuffer(capacity, dynamic);

    public override NativeRenderTarget CreateRenderTarget(IReadOnlyList<NativeRenderBuffer> color, NativeRenderBuffer? depth = null)
    {
        return new NullRenderTarget(color, depth);
    }

    protected override NativeShader CreateShader(ShaderTranspileResult result) => new NullShader();

    protected override ShaderTranspiler CreateShaderTranspiler() => new NullShaderTranspiler();

    public override NativeTexture CreateTexture(int width, int height, int depth, int levels, int layers, FilterMode min, FilterMode mag, WrapMode wrapModeS, WrapMode wrapModeT, WrapMode wrapModeR, TextureType type, TextureUsage usage, TextureSampleCount sampleCount, PixelFormat format)
        => new NullTexture(width, height, depth, layers, levels, min, mag, wrapModeS, wrapModeT, wrapModeR, format, type, usage, sampleCount);

    public override void Draw(uint vertexCount, PrimitiveTopology topology, uint instanceCount, int baseVertex, uint baseInstance)
    {
    }

    public override void DrawIndexed(uint indexCount, PrimitiveTopology topology, uint instanceCount, uint indexOffset, int baseVertex, uint baseInstance)
    {
    }

    public override void DrawIndexedIndirect(NativeBuffer buffer, PrimitiveTopology topology, uint offset, uint drawCount)
    {
    }

    public override void DrawIndirect(NativeBuffer buffer, PrimitiveTopology topology, uint offset, uint drawCount)
    {
    }

    public override void MakeCurrent()
    {
    }

    public override void Prepare()
    {
    }

    public override void Present()
    {
    }

    public override void SetBlend(BlendInfo blending)
    {
    }

    public override void SetBuffer(NativeBuffer? buffer = null, IndexFormat format = IndexFormat.UInt16)
    {
    }

    public override void SetBuffer(NativeBuffer? buffer = null, IVertexLayout? layout = null)
    {
    }

    public override void SetDepth(DepthInfo depth)
    {
    }

    public override void SetFaceCulling(FaceCulling culling)
    {
    }

    public override void SetFaceWinding(FaceWinding winding)
    {
    }

    public override void SetRenderTarget(NativeRenderTarget? buffer = null)
    {
    }

    public override void SetScissor(ScissorInfo scissor)
    {
    }

    public override void SetShader(NativeShader? shader = null)
    {
    }

    public override void SetStencil(StencilInfo stencil)
    {
    }

    public override void SetTexture(NativeTexture? texture = null, int unit = 0)
    {
    }

    public override void SetViewport(Viewport viewport)
    {
    }
}
