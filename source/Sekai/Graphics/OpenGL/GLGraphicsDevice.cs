// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Platform;
using Silk.NET.OpenGL;

namespace Sekai.Graphics.OpenGL;

internal sealed unsafe class GLGraphicsDevice : GraphicsDevice
{
    public override GraphicsAPI API => GraphicsAPI.OpenGL;
    public override GraphicsAdapter Adapter { get; }
    internal Rectangle Viewport { get; private set; }

    public override SyncMode SyncMode
    {
        get => syncMode;
        set
        {
            if (value == SyncMode.Adaptive)
            {
                if (!Adapter.IsFeatureSupported(GraphicsDeviceFeatures.AdaptiveSyncMode))
                {
                    syncMode = value;
                }
                else
                {
                    syncMode = SyncMode.Traditional;
                }
            }

            source.SetSyncToVerticalBlank(syncMode);
        }
    }

    private readonly uint vao;
    private readonly nint context;
    private readonly int defaultFramebufferId;
    private readonly IGLContextSource source;
    private SyncMode syncMode;
    private bool isDisposed;
    private uint enabledAttributeCount;
    private VertexLayout currentLayout;
    private DrawElementsType drawElementsType;

#pragma warning disable IDE1006

    private readonly GL GL;

#pragma warning restore IDE1006

    public GLGraphicsDevice(IGLContextSource source)
    {
        GL = GL.GetApi(source.GetProcAddress);

        context = source.CreateContext();
        Adapter = new GLGraphicsAdapter(GL);

        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        GL.GetInteger(GetPName.DrawFramebufferBinding, out defaultFramebufferId);

        this.source = source;
    }

    public override void Clear(ClearInfo info)
    {
        uint mask = 0;

        if ((info.Flags & ClearFlags.Color) == ClearFlags.Color)
        {
            mask |= (uint)ClearBufferMask.ColorBufferBit;
            GL.ClearColor(info.Color);
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

    public override Framebuffer CreateFramebuffer(FramebufferAttachment? depth, params FramebufferAttachment[] colors)
    {
        return new GLFramebuffer(GL, depth, colors);
    }

    public override RasterizerState CreateRasterizerState(RasterizerStateDescription description)
    {
        return new GLRasterizerState(description);
    }

    public override Sampler CreateSampler(SamplerDescription description)
    {
        return new GLSampler(GL, description);
    }

    public override Shader CreateShader(params ShaderCode[] attachments)
    {
        return new GLShader(GL, attachments);
    }

    public override Texture CreateTexture(TextureDescription description)
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

    ~GLGraphicsDevice()
    {
        Dispose();
    }

    public override void Dispose()
    {
        if (isDisposed)
        {
            return;
        }

        GL.BindVertexArray(0);
        GL.DeleteVertexArray(vao);
        source.ClearCurrentContext();
        source.DeleteContext(context);

        isDisposed = true;

        GC.SuppressFinalize(this);
    }

    public override void Draw(PrimitiveType type, uint vertexCount, uint vertexStart, uint instanceCount, uint instanceStart)
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

    public override void DrawIndexed(PrimitiveType type, uint indexCount, uint indexStart, uint vertexStart, uint instanceCount, uint instanceStart)
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

    public override void DrawIndirect(GraphicsBuffer buffer, PrimitiveType type, uint offset, uint drawCount, uint stride)
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

    public override void DrawIndexedIndirect(GraphicsBuffer buffer, PrimitiveType type, uint offset, uint drawCount, uint stride)
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

    public override void SetFramebuffer(Framebuffer? framebuffer)
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

    public override void SetShader(Shader shader)
    {
        ((GLShader)shader).Bind();
    }

    public override void SetTexture(Texture texture, uint slot)
    {
        ((GLTexture)texture).Bind(slot);
    }

    public override void SetSampler(Sampler sampler, uint slot)
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

    public override void SetVertexBuffer(GraphicsBuffer buffer, VertexLayout layout)
    {
        ((GLBuffer)buffer).Bind();

        if (currentLayout.Equals(layout))
        {
            return;
        }

        currentLayout = layout;

        if (currentLayout.Members.Count > enabledAttributeCount)
        {
            for (uint i = enabledAttributeCount; i < currentLayout.Members.Count; i++)
                GL.EnableVertexAttribArray(i);
        }
        else
        {
            for (uint i = enabledAttributeCount - 1; i >= currentLayout.Members.Count; i++)
                GL.DisableVertexAttribArray(i);
        }

        enabledAttributeCount = (uint)currentLayout.Members.Count;

        for (uint i = 0; i < currentLayout.Members.Count; i++)
        {
            var member = currentLayout.Members[(int)i];
            GL.VertexAttribPointer(i, member.Count, member.Format.AsAttribType(), member.Normalized, currentLayout.Stride, (void*)member.Offset);
        }
    }

    public override void SetViewport(Rectangle viewport)
    {
        GL.Viewport(Viewport = viewport);
    }
}
