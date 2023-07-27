// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Sekai.Mathematics;

namespace Sekai.Headless;

internal sealed class HeadlessGraphicsDevice : GraphicsDevice
{
    public override GraphicsAPI API => GraphicsAPI.Dummy;
    public override string Device { get; } = "Dummy";
    public override string Vendor { get; } = "Dummy";
    public override Version Version { get; } = new();
    public override bool SyncMode { get; set; }

    public override void MakeCurrent()
    {
    }

    public override void Clear(ClearInfo info)
    {
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new HeadlessBlendState();
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new HeadlessInputLayout();
    }

    public override GraphicsBuffer CreateBuffer(BufferType type, uint size, bool dynamic = false)
    {
        return new HeadlessGraphicsBuffer((int)size, type, dynamic);
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new HeadlessDepthStencilState();
    }

    public override Framebuffer CreateFramebuffer(FramebufferAttachment? depth, params FramebufferAttachment[] color)
    {
        return new HeadlessFramebuffer();
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new HeadlessRasterizerState();
    }

    public override Sampler CreateSampler(SamplerDescription description)
    {
        return new HeadlessSampler(description);
    }

    public override Shader CreateShader(params ShaderCode[] attachments)
    {
        return new HeadlessShader(attachments);
    }

    public override Texture CreateTexture(TextureDescription description)
    {
        return new HeadlessTexture(description);
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
    }

    public override void DispatchIndirect(GraphicsBuffer buffer, uint offset)
    {
    }

    public override void Dispose()
    {
    }

    public override void Draw(PrimitiveType type, uint vertexCount, uint vertexStart, uint instanceCount, uint instanceStart)
    {
    }

    public override void DrawIndexed(PrimitiveType type, uint indexCount, uint indexStart, uint vertexStart, uint instanceCount, uint instanceStart)
    {
    }

    public override void DrawIndexedIndirect(GraphicsBuffer buffer, PrimitiveType type, uint offset, uint drawCount, uint stride)
    {
    }

    public override void DrawIndirect(GraphicsBuffer buffer, PrimitiveType type, uint offset, uint drawCount, uint stride)
    {
    }

    public override void Flush()
    {
    }

    public override void Present()
    {
    }

    public override void SetBlendState(BlendState state)
    {
    }

    public override void SetDepthStencilState(DepthStencilState state, int reference)
    {
    }

    public override void SetFramebuffer(Framebuffer? framebuffer)
    {
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
    }

    public override void SetRasterizerState(RasterizerState state)
    {
    }

    public override void SetScissor(Rectangle scissor)
    {
    }

    public override void SetShader(Shader shader)
    {
    }

    public override void SetTexture(Texture texture, Sampler sampler, uint slot)
    {
    }

    public override void SetUniformBuffer(GraphicsBuffer buffer, uint slot)
    {
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
    }

    public override void SetViewport(Rectangle viewport)
    {
    }
}
