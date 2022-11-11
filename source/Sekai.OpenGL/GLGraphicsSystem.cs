// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Graphics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;
using Sekai.Windowing;
using Sekai.Windowing.OpenGL;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal class GLGraphicsSystem : FrameworkObject, IGraphicsSystem
{
    internal GL GL { get; private set; } = null!;
    private IOpenGLProvider provider = null!;
    private int enabledAttributeCount;
    private bool? lastBlendingState;
    private IndexFormat indexFormat;
    private Rectangle lastViewport;
    private uint vao;

    public IGraphicsFactory CreateFactory() => new GLGraphicsFactory(this);

    public void Initialize(IView view)
    {
        if (view is not IOpenGLProviderSource source)
            throw new Exception($"Window system is not a GL provider.");

        GL = GL.GetApi(source.GL.GetProcAddress);
        provider = source.GL;

        vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);
    }

    public void Present()
    {
        provider.SwapBuffers();
    }

    public void Reset()
    {
        lastBlendingState = null;
        lastViewport = Rectangle.Empty;
    }

    public void Clear(ClearInfo info)
    {
        GL.ClearColor(info.Color.R, info.Color.G, info.Color.B, info.Color.A);
        GL.ClearDepth(info.Depth);
        GL.ClearStencil(info.Stencil);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }

    public void SetDepth(DepthInfo info)
    {
        if (info.DepthTest)
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(GLUtils.ToDepthFunction(info.Function));
        }
        else
        {
            GL.Disable(EnableCap.DepthTest);
        }

        GL.DepthMask(info.WriteDepth);
    }

    public void SetScissor(Rectangle scissor)
    {
        GL.Scissor(scissor.X, lastViewport.Height - scissor.Bottom, (uint)scissor.Width, (uint)scissor.Height);
    }

    public void SetScissorState(bool state)
    {
        if (state)
        {
            GL.Enable(EnableCap.ScissorTest);
        }
        else
        {
            GL.Disable(EnableCap.ScissorTest);
        }
    }

    public void SetStencil(StencilInfo info)
    {
        if (info.StencilTest)
        {
            GL.Enable(EnableCap.StencilTest);
            GL.StencilFunc(GLUtils.ToStencilFunction(info.Function), info.Value, (uint)info.Mask);
            GL.StencilOp
            (
                GLUtils.ToStencilOperation(info.StencilFailOperation),
                GLUtils.ToStencilOperation(info.DepthTestFailOperation),
                GLUtils.ToStencilOperation(info.PassOperation)
            );
        }
        else
        {
            GL.Disable(EnableCap.StencilTest);
        }
    }

    public void SetViewport(Rectangle viewport)
    {
        lastViewport = viewport;
        GL.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
    }

    public void SetBlend(BlendingParameters parameters)
    {
        if (parameters.IsDisabled)
        {
            if (!lastBlendingState.HasValue || lastBlendingState.Value)
                GL.Disable(EnableCap.Blend);

            lastBlendingState = false;
        }
        else
        {
            if (!lastBlendingState.HasValue || !lastBlendingState.Value)
                GL.Enable(EnableCap.Blend);

            lastBlendingState = true;

            GL.BlendEquationSeparate
            (
                GLUtils.ToBlendEquationModeEXT(parameters.ColorEquation),
                GLUtils.ToBlendEquationModeEXT(parameters.AlphaEquation)
            );

            GL.BlendFuncSeparate
            (
                GLUtils.ToBlendingFactor(parameters.SourceColor),
                GLUtils.ToBlendingFactor(parameters.DestinationColor),
                GLUtils.ToBlendingFactor(parameters.SourceAlpha),
                GLUtils.ToBlendingFactor(parameters.DestinationAlpha)
            );
        }
    }

    public void SetBlendMask(BlendingMask mask)
    {
        GL.ColorMask
        (
            mask.HasFlag(BlendingMask.Red),
            mask.HasFlag(BlendingMask.Green),
            mask.HasFlag(BlendingMask.Blue),
            mask.HasFlag(BlendingMask.Alpha)
        );
    }

    public void SetTexture(INativeTexture? texture, int unit)
    {
        if (texture is null)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
        else
        {
            var tex = (GLTexture)texture;
            GL.ActiveTexture(TextureUnit.Texture0 + unit);
            GL.BindTexture(tex.Target, tex);
        }
    }

    public void SetShader(INativeShader shader)
    {
        GL.UseProgram((GLShader)shader);
    }

    public void SetIndexBuffer(INativeBuffer buffer, IndexFormat format)
    {
        indexFormat = format;
        GL.BindBuffer(BufferTargetARB.ElementArrayBuffer, (GLBuffer)buffer);
    }

    public unsafe void SetVertexBuffer(INativeBuffer buffer, IVertexLayout layout)
    {
        if (layout.Members.Count > enabledAttributeCount)
        {
            for (int i = enabledAttributeCount; i < layout.Members.Count; i++)
                GL.EnableVertexAttribArray((uint)i);
        }
        else
        {
            for (int i = enabledAttributeCount - 1; i >= layout.Members.Count; i++)
                GL.DisableVertexAttribArray((uint)i);
        }

        enabledAttributeCount = layout.Members.Count;

        GL.BindBuffer(BufferTargetARB.ArrayBuffer, (GLBuffer)buffer);

        for (int i = 0; i < layout.Members.Count; i++)
        {
            var member = layout.Members[i];
            GL.VertexAttribPointer((uint)i, member.Count, member.Format.ToVertexAttribPointerType(), member.Normalized, (uint)(member.Count * member.Format.SizeOfFormat()), (void*)member.Offset);
        }
    }

    public void SetFrameBuffer(INativeFrameBuffer? buffer)
    {
        if (buffer is null)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }
        else
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, (GLFrameBuffer)buffer);

            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
                throw new InvalidOperationException(@"Framebuffer is not ready to be bound.");
        }
    }

    public unsafe void Draw(int count, PrimitiveTopology topology)
    {
        var mode = topology switch
        {
            PrimitiveTopology.Lines => PrimitiveType.Lines,
            PrimitiveTopology.LineStrip => PrimitiveType.LineStrip,
            PrimitiveTopology.Points => PrimitiveType.Points,
            PrimitiveTopology.Triangles => PrimitiveType.Triangles,
            PrimitiveTopology.TriangleStrip => PrimitiveType.TriangleStrip,
            _ => throw new NotSupportedException($@"Primitive topology ""{topology}"" is not supported.")
        };

        var type = indexFormat switch
        {
            IndexFormat.UInt16 => DrawElementsType.UnsignedShort,
            IndexFormat.UInt32 => DrawElementsType.UnsignedInt,
            _ => throw new NotSupportedException($@"Index format ""{indexFormat}"" is not supported.")
        };

        GL.DrawElements(mode, (uint)count, type, null);
    }

    protected override void Destroy()
    {
        if (vao != 0)
            GL.DeleteVertexArray(vao);
    }
}
