// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Contexts;
using Sekai.Graphics;
using Sekai.Mathematics;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal sealed unsafe class GLGraphicsDevice : GraphicsDevice
{
    public override GraphicsAPI API => GraphicsAPI.OpenGL;
    public override string Device { get; }
    public override string Vendor { get; }
    public override Version Version { get; }
    internal Rectangle Viewport { get; private set; }

    public override bool SyncMode
    {
        get => syncMode;
        set => source.SwapInterval((syncMode = value) ? 1 : 0);
    }

    private readonly uint vao;
    private readonly int defaultFramebufferId;
    private readonly GLContext source;
    private bool syncMode;
    private bool isDisposed;
    private int enabledAttributeCount;
    private DrawElementsType drawElementsType;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLGraphicsDevice(GLContext source)
    {
        GL = GL.GetApi((this.source = source).GetProcAddress);

        MakeCurrent();

        GL.GetInteger(GetPName.MajorVersion, out int major);
        GL.GetInteger(GetPName.MinorVersion, out int minor);
        GL.GetInteger(GetPName.DrawFramebufferBinding, out defaultFramebufferId);

        Device = GL.GetStringS(StringName.Renderer);
        Vendor = GL.GetStringS(StringName.Vendor);
        Version = new(major, minor);

        GL.GenVertexArrays(1, out vao);
        GL.BindVertexArray(vao);
    }

    public override void MakeCurrent()
    {
        source.MakeCurrent();
    }

    public override void Clear(ClearInfo info)
    {
        uint mask = 0;

        if ((info.Flags & ClearFlags.Color) == ClearFlags.Color)
        {
            mask |= (uint)ClearBufferMask.ColorBufferBit;
            var color = info.Color.ToColor4();
            GL.ClearColor(color.R, color.G, color.B, color.A);
        }

        if ((info.Flags & ClearFlags.Depth) == ClearFlags.Depth)
        {
            mask |= (uint)ClearBufferMask.DepthBufferBit;
            GL.ClearDepth(info.Depth);
        }

        if ((info.Flags & ClearFlags.Stencil) == ClearFlags.Stencil)
        {
            mask |= (uint)ClearBufferMask.StencilBufferBit;
            GL.ClearStencil(info.Stencil);
        }

        GL.Clear(mask);
    }

    public override InputLayout CreateInputLayout(params InputLayoutDescription[] descriptions)
    {
        return new GLInputLayout(descriptions);
    }

    public override BlendState CreateBlendState(BlendStateDescription description)
    {
        return new GLBlendState(description);
    }

    public override GraphicsBuffer CreateBuffer(BufferType type, uint size, bool dynamic = false)
    {
        return new GLBuffer(GL, type, size, dynamic);
    }

    public override DepthStencilState CreateDepthStencilState(DepthStencilStateDescription description)
    {
        return new GLDepthStencilState(description);
    }

    public override Graphics.Framebuffer CreateFramebuffer(Graphics.FramebufferAttachment? depth, params Graphics.FramebufferAttachment[] colors)
    {
        return new GLFramebuffer(GL, depth, colors);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new GLRasterizerState(description);
    }

    public override Graphics.Sampler CreateSampler(SamplerDescription description)
    {
        return new GLSampler(GL, description);
    }

    public override Graphics.Shader CreateShader(params ShaderCode[] attachments)
    {
        return new GLShader(GL, attachments);
    }

    public override Graphics.Texture CreateTexture(TextureDescription description)
    {
        return new GLTexture(GL, description);
    }

    public override void Dispatch(uint groupCountX, uint groupCountY, uint groupCountZ)
    {
        GL.DispatchCompute(groupCountX, groupCountY, groupCountZ);
    }

    public override void DispatchIndirect(GraphicsBuffer buffer, uint offset)
    {
        ((GLBuffer)buffer).Bind();
        GL.DispatchComputeIndirect((nint)offset);
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        MakeCurrent();

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(vao);
        source.Clear();

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    public override void Draw(Graphics.PrimitiveType type, uint vertexCount, uint vertexStart, uint instanceCount, uint instanceStart)
    {
        if (instanceCount == 1 && instanceStart == 0)
        {
            GL.DrawArrays(type.AsPrimitiveType(), (int)vertexStart, vertexCount);
        }
        else
        {
            if (instanceStart == 0)
            {
                GL.DrawArraysInstanced(type.AsPrimitiveType(), (int)vertexStart, vertexCount, instanceCount);
            }
            else
            {
                GL.DrawArraysInstancedBaseInstance(type.AsPrimitiveType(), (int)vertexStart, vertexCount, instanceCount, instanceStart);
            }
        }
    }

    public override void DrawIndexed(Graphics.PrimitiveType type, uint indexCount, uint indexStart, uint vertexStart, uint instanceCount, uint instanceStart)
    {
        uint indexSize = drawElementsType == DrawElementsType.UnsignedShort ? 2u : 4u;
        void* offset = (void*)(indexStart * indexSize);

        if (instanceCount == 1 && instanceStart == 0)
        {
            if (vertexStart == 0)
            {
                GL.DrawElements(type.AsPrimitiveType(), indexCount, drawElementsType, offset);
            }
            else
            {
                GL.DrawElementsBaseVertex(type.AsPrimitiveType(), indexCount, drawElementsType, offset, (int)vertexStart);
            }
        }
        else
        {
            if (instanceStart > 0 && vertexStart > 0)
            {
                GL.DrawElementsInstancedBaseVertexBaseInstance(type.AsPrimitiveType(), indexCount, drawElementsType, offset, instanceCount, (int)vertexStart, instanceStart);
            }
            else if (instanceStart > 0 && vertexStart == 0)
            {
                GL.DrawElementsInstancedBaseInstance(type.AsPrimitiveType(), indexCount, drawElementsType, offset, instanceCount, instanceStart);
            }
            else if (instanceStart == 0 && vertexStart > 0)
            {
                GL.DrawElementsInstancedBaseVertex(type.AsPrimitiveType(), indexCount, drawElementsType, offset, instanceCount, (int)vertexStart);
            }
            else
            {
                GL.DrawElementsInstanced(type.AsPrimitiveType(), indexCount, drawElementsType, offset, instanceCount);
            }
        }
    }

    public override void DrawIndirect(GraphicsBuffer buffer, Graphics.PrimitiveType type, uint offset, uint drawCount, uint stride)
    {
        ((GLBuffer)buffer).Bind();

        if (drawCount > 0)
        {
            GL.MultiDrawArraysIndirect(type.AsPrimitiveType(), (void*)offset, drawCount, stride);
        }
        else
        {
            GL.DrawArraysIndirect(type.AsPrimitiveType(), (void*)offset);
        }
    }

    public override void DrawIndexedIndirect(GraphicsBuffer buffer, Graphics.PrimitiveType type, uint offset, uint drawCount, uint stride)
    {
        ((GLBuffer)buffer).Bind();

        if (drawCount > 0)
        {
            GL.MultiDrawElementsIndirect(type.AsPrimitiveType(), drawElementsType, (void*)offset, drawCount, stride);
        }
        else
        {
            GL.DrawElementsIndirect(type.AsPrimitiveType(), drawElementsType, (void*)offset);
        }
    }

    public override void Flush()
    {
        GL.Flush();
    }

    public override void Present()
    {
        source.SwapBuffers();
    }

    public override void SetBlendState(BlendState state)
    {
        var s = (GLBlendState)state;

        GL.ColorMask(s.Red, s.Green, s.Blue, s.Alpha);

        if (!s.Enabled)
        {
            GL.Disable(EnableCap.Blend);
            return;
        }

        GL.Enable(EnableCap.Blend);
        GL.BlendFuncSeparate(s.SourceColor, s.DestinationColor, s.SourceAlpha, s.DestinationAlpha);
        GL.BlendEquationSeparate(s.ColorEquation, s.AlphaEquation);
    }

    public override void SetDepthStencilState(DepthStencilState state, int reference)
    {
        var s = (GLDepthStencilState)state;

        if (s.DepthEnabled)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(s.DepthMask);
            GL.DepthFunc(s.DepthFunction);
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }

        if (s.StencilEnabled)
        {
            GL.Enable(EnableCap.StencilTest);
            GL.StencilOpSeparate(TriangleFace.Back, s.BackStencilFail, s.BackDepthFail, s.BackStencilPass);
            GL.StencilOpSeparate(TriangleFace.Front, s.FrontStencilFail, s.FrontDepthFail, s.FrontStencilPass);
            GL.StencilMask(s.StencilWriteMask);
            GL.StencilFuncSeparate(TriangleFace.Back, s.BackFunction, reference, s.StencilReadMask);
            GL.StencilFuncSeparate(TriangleFace.Front, s.FrontFunction, reference, s.StencilReadMask);
        }
        else
        {
            GL.Disable(EnableCap.StencilTest);
        }
    }

    public override void SetFramebuffer(Graphics.Framebuffer? framebuffer)
    {
        if (framebuffer is null)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, (uint)defaultFramebufferId);
        }
        else
        {
            ((GLFramebuffer)framebuffer).Bind();
        }
    }

    public override void SetRasterizerState(RasterizerState state)
    {
        var s = (GLRasterizerState)state;

        if (!s.Culling)
        {
            GL.Disable(EnableCap.CullFace);
        }
        else
        {
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(s.CullingMode);
        }

        if (s.Scissor)
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        else
        {
            GL.Disable(EnableCap.ScissorTest);
        }

        GL.FrontFace(s.FrontFace);
        GL.PolygonMode(TriangleFace.FrontAndBack, s.PolygonMode);
    }

    public override void SetScissor(Rectangle scissor)
    {
        GL.Scissor(scissor.X, Viewport.Height - scissor.Bottom, (uint)scissor.Width, (uint)scissor.Height);
    }

    public override void SetShader(Graphics.Shader shader)
    {
        ((GLShader)shader).Bind();
    }

    public override void SetTexture(Graphics.Texture texture, uint slot)
    {
        ((GLTexture)texture).Bind(slot);
    }

    public override void SetSampler(Graphics.Sampler sampler, uint slot)
    {
        ((GLSampler)sampler).Bind(slot);
    }

    public override void SetIndexBuffer(GraphicsBuffer buffer, IndexType type)
    {
        drawElementsType = type.AsElementsType();
        ((GLBuffer)buffer).Bind();
    }

    public override void SetUniformBuffer(GraphicsBuffer buffer, uint slot)
    {
        ((GLBuffer)buffer).Bind(slot);
    }

    public override void SetVertexBuffer(GraphicsBuffer buffer, InputLayout layout)
    {
        ((GLBuffer)buffer).Bind();

        var input = (GLInputLayout)layout;

        int currentAttrib = 0;
        int currentOffset = 0;

        for (int slot = 0; slot < input.Descriptions.Count; slot++)
        {
            var description = input.Descriptions[slot];

            for (int i = 0; i < description.Members.Count; i++)
            {
                var member = description.Members[i];
                int actual = slot + i;

                if (actual >= enabledAttributeCount)
                {
                    GL.EnableVertexAttribArray((uint)actual);
                }

                GL.VertexAttribPointer((uint)actual, member.Count, member.Format.AsAttribType(), member.Normalized, (uint)input.Strides[slot], (void*)currentOffset);

                currentOffset += member.Format.SizeOfFormat() * member.Count;
            }

            currentAttrib += description.Members.Count;
        }

        for (int i = currentAttrib; i < enabledAttributeCount; i++)
        {
            GL.DisableVertexAttribArray((uint)i);
        }

        enabledAttributeCount = currentAttrib;
    }

    public override void SetViewport(Rectangle viewport)
    {
        GL.Viewport(Viewport = viewport);
    }
}
