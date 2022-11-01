// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework.Graphics;
using Sekai.Framework.Windowing;
using Sekai.Framework.Windowing.OpenGL;
using Silk.NET.OpenGL;

namespace Sekai.OpenGL;

internal class GLGraphicsContext : GraphicsContext
{
    internal GL GL { get; private set; } = null!;
    private IOpenGLProvider provider = null!;

    protected override void InitializeImpl(IView view)
    {
        if (view is not IOpenGLProviderSource source)
            throw new ArgumentException($"Window system is not a GL provider.", nameof(view));

        GL = GL.GetApi(source.GL.GetProcAddress);
        provider = source.GL;
    }

    protected override void PrepareImpl()
    {
        provider.MakeCurrent();
    }

    protected override  void PresentImpl()
    {
        provider.SwapBuffers();
        provider.ClearCurrentContext();
    }

    protected override void FinishImpl()
    {
    }

    protected override void ClearImpl(ClearInfo info)
    {
        if (info.Color != CurrentClearInfo.Color)
            GL.ClearColor(info.Color.R, info.Color.G, info.Color.B, info.Color.A);

        if (info.Depth != CurrentClearInfo.Depth)
            GL.ClearDepth(info.Depth);

        if (info.Stencil != CurrentClearInfo.Stencil)
            GL.ClearStencil(info.Stencil);

        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
    }

    protected override void SetDepth(DepthInfo info)
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

    protected override void SetScissor(Rectangle scissor)
    {
        GL.Scissor(scissor.X, scissor.Y, (uint)scissor.Width, (uint)scissor.Height);
    }

    protected override void SetScissorState(bool state)
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

    protected override void SetStencil(StencilInfo info)
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

    protected override void SetViewport(Rectangle viewport)
    {
        GL.Viewport(viewport.X, viewport.Y, (uint)viewport.Width, (uint)viewport.Height);
    }

    protected override void SetBlendImpl(BlendingParameters parameters)
    {
    }

    protected override void SetBlendMaskImpl(BlendingMask mask)
    {
        GL.ColorMask
        (
            mask.HasFlag(BlendingMask.Red),
            mask.HasFlag(BlendingMask.Green),
            mask.HasFlag(BlendingMask.Blue),
            mask.HasFlag(BlendingMask.Alpha)
        );
    }

    protected override void Destroy()
    {
        if (!HasInitialized)
            return;

        GL.Dispose();
    }
}
