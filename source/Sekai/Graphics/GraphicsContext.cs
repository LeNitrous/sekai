// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Graphics.Buffers;
using Sekai.Graphics.Shaders;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;
using Sekai.Windowing;

namespace Sekai.Graphics;

public sealed class GraphicsContext : FrameworkObject
{
    /// <summary>
    /// The current clear info.
    /// </summary>
    public ClearInfo CurrentClear { get; private set; }

    /// <summary>
    /// The current depth info.
    /// </summary>
    public DepthInfo CurrentDepth { get; private set; }

    /// <summary>
    /// The current stencil info.
    /// </summary>
    public StencilInfo CurrentStencil { get; private set; }

    /// <summary>
    /// The current viewport.
    /// </summary>
    public Viewport CurrentViewport { get; private set; }

    /// <summary>
    /// The current scissor rectangle.
    /// </summary>
    public Rectangle CurrentScissor { get; private set; }

    /// <summary>
    /// The current scissor state.
    /// </summary>
    public bool CurrentScissorState { get; private set; }

    /// <summary>
    /// The current blending mask.
    /// </summary>
    public BlendingMask CurrentBlendMask { get; private set; }

    /// <summary>
    /// The current blending parameters.
    /// </summary>
    public BlendingParameters CurrentBlend { get; private set; }

    /// <summary>
    /// The current face winding.
    /// </summary>
    public FaceWinding CurrentFaceWinding { get; private set; }

    /// <summary>
    /// The current face culling.
    /// </summary>
    public FaceCulling CurrentFaceCulling { get; private set; }

    /// <summary>
    /// The current shader.
    /// </summary>
    public Shader? CurrentShader { get; private set; }

    /// <summary>
    /// The current vertex buffer.
    /// </summary>
    public Buffers.Buffer? CurrentVertexBuffer { get; private set; }

    /// <summary>
    /// The current index buffer.
    /// </summary>
    public Buffers.Buffer? CurrentIndexBuffer { get; private set; }

    /// <summary>
    /// The current frame buffer.
    /// </summary>
    public FrameBuffer? CurrentFrameBuffer { get; private set; }

    /// <summary>
    /// The current projection matrix.
    /// </summary>
    public Matrix4x4 CurrentProjectionMatrix  { get; private set; }

    /// <summary>
    /// The default render target.
    /// </summary>
    public IRenderTarget BackBufferTarget => new BackBufferRenderTarget(this);

    /// <summary>
    /// The default texture.
    /// </summary>
    public Texture WhitePixel { get; }

    /// <summary>
    /// The underlying windowing system.
    /// </summary>
    public readonly IView View;

    /// <summary>
    /// The underlying graphics system.
    /// </summary>
    public readonly IGraphicsSystem Graphics;

    /// <summary>
    /// The underlying graphics resource factory.
    /// </summary>
    public readonly IGraphicsFactory Factory;

    /// <summary>
    /// The global uniforms for this graphics context.
    /// </summary>
    public readonly GlobalUniformManager Uniforms = new();

    private readonly Queue<Action> disposals = new();
    private readonly Stack<StencilInfo> stencilStack = new();
    private readonly Stack<DepthInfo> depthStack = new();
    private readonly Stack<Viewport> viewportStack = new();
    private readonly Stack<Rectangle> scissorStack = new();
    private readonly Stack<bool> scissorStateStack = new();
    private readonly Stack<Shader> shaderStack = new();
    private readonly Stack<FrameBuffer?> frameBufferStack = new();
    private readonly Stack<Matrix4x4> projMatrixStack = new();
    private readonly Texture[] boundTextures = new Texture[16];
    private int prevBoundTextureUnit = -1;
    private IndexFormat? prevBoundIndexFormat;
    private IVertexLayout? prevBoundVertexLayout;

    internal GraphicsContext(IGraphicsSystem graphics, IView view)
    {
        View = view;

        Graphics = graphics;
        Graphics.Initialize(view);

        Factory = graphics.CreateFactory();

        WhitePixel = new Texture(this, Factory.CreateTexture(1, 1, 1, 1, 1, FilterMode.Nearest, FilterMode.Nearest, WrapMode.Repeat, WrapMode.Repeat, WrapMode.Repeat, TextureType.Texture2D, TextureUsage.Sampled, TextureSampleCount.Count1, PixelFormat.R8_G8_B8_A8_UNorm_SRgb));
        WhitePixel.SetData(new byte[] { 255, 255, 255, 255 }, 0, 0, 0, 1, 1, 1, 0, 0);
    }

    /// <summary>
    /// Prepare for the next frame.
    /// </summary>
    public void Prepare()
    {
        while (disposals.TryDequeue(out var dispose))
            dispose();

        Reset();
        Clear(new ClearInfo(new Color4(0, 0, 0, 1f)));
    }

    /// <summary>
    /// Presents the current frame to the window.
    /// </summary>
    public void Present()
    {
        Graphics.Present();
    }

    /// <summary>
    /// Resets the state of the graphics context.
    /// </summary>
    public void Reset()
    {
        CurrentBlend = new();
        CurrentShader = null;
        CurrentScissor = Rectangle.Empty;
        CurrentViewport = Viewport.Empty;
        CurrentProjectionMatrix = Matrix4x4.Identity;
        CurrentFrameBuffer = null;
        CurrentIndexBuffer = null;
        CurrentVertexBuffer = null;
        CurrentScissorState = false;
        prevBoundTextureUnit = -1;
        prevBoundIndexFormat = null;
        prevBoundVertexLayout = null;

        depthStack.Clear();
        shaderStack.Clear();
        stencilStack.Clear();
        scissorStack.Clear();
        viewportStack.Clear();
        projMatrixStack.Clear();
        scissorStateStack.Clear();
        boundTextures.AsSpan().Clear();

        Graphics.Reset();

        PushScissorState(true);
        PushViewport(new Viewport(0, 0, View.Size.Width, View.Size.Height, 0, 1));
        PushScissor(new Rectangle(0, 0, View.Size.Width, View.Size.Height));
        PushDepth(new DepthInfo(true));
        PushStencil(new StencilInfo(false));
    }

    /// <summary>
    /// Clears the current framebuffer.
    /// </summary>
    public void Clear(ClearInfo info)
    {
        PushDepth(new DepthInfo(writeDepth: true));
        PushScissorState(false);

        Graphics.Clear(info);

        PopScissorState();
        PopDepth();
    }

    /// <summary>
    /// Sets the current blending parameters.
    /// </summary>
    public void SetBlend(BlendingParameters blend)
    {
        if (CurrentBlend == blend)
            return;

        Graphics.SetBlend(blend);
        CurrentBlend = blend;
    }

    /// <summary>
    /// Sets the current blending mask.
    /// </summary>
    public void SetBlendMask(BlendingMask mask)
    {
        if (CurrentBlendMask == mask)
            return;

        Graphics.SetBlendMask(mask);
        CurrentBlendMask = mask;
    }

    /// <summary>
    /// Sets the current face winding.
    /// </summary>
    public void SetFaceWinding(FaceWinding winding)
    {
        if (CurrentFaceWinding == winding)
            return;

        Graphics.SetFaceWinding(winding);
        CurrentFaceWinding = winding;
    }

    /// <summary>
    /// Sets the current face culling.
    /// </summary>
    public void SetFaceCulling(FaceCulling culling)
    {
        if (CurrentFaceCulling == culling)
            return;

        Graphics.SetFaceCulling(culling);
        CurrentFaceCulling = culling;
    }

    /// <summary>
    /// Applies new depth parameters.
    /// </summary>
    public void PushDepth(DepthInfo depth)
    {
        depthStack.Push(depth);
        setDepth(depth);
    }

    /// <summary>
    /// Restores the previous depth parameters.
    /// </summary>
    public void PopDepth()
    {
        depthStack.Pop();
        setDepth(depthStack.Peek());
    }

    /// <summary>
    /// Applies a new viewport rectangle.
    /// </summary>
    public void PushViewport(Viewport viewport)
    {
        viewportStack.Push(viewport);
        setViewport(viewport);
    }

    /// <summary>
    /// Restores the previous viewport rectangle.
    /// </summary>
    public void PopViewport()
    {
        viewportStack.Pop();
        setViewport(viewportStack.Peek());
    }

    /// <summary>
    /// Applies a new scissor rectangle.
    /// </summary>
    public void PushScissor(Rectangle scissor)
    {
        scissorStack.Push(scissor);
        setScissor(scissor);
    }

    /// <summary>
    /// Restores the previous scissor rectangle.
    /// </summary>
    public void PopScissor()
    {
        scissorStack.Pop();
        setScissor(scissorStack.Peek());
    }

    /// <summary>
    /// Applies a new scissor state.
    /// </summary>
    public void PushScissorState(bool enabled)
    {
        scissorStateStack.Push(enabled);
        setScissorState(enabled);
    }

    /// <summary>
    /// Restores the previous scissor state.
    /// </summary>
    public void PopScissorState()
    {
        scissorStateStack.Pop();
        setScissorState(scissorStateStack.Peek());
    }

    /// <summary>
    /// Applies the new stencil parameters.
    /// </summary>
    public void PushStencil(StencilInfo stencil)
    {
        stencilStack.Push(stencil);
        setStencil(stencil);
    }

    /// <summary>
    /// Restores the previous stencil parameters.
    /// </summary>
    public void PopStencil()
    {
        stencilStack.Pop();
        setStencil(stencilStack.Peek());
    }

    /// <summary>
    /// Draws vertices onto the current framebuffer.
    /// </summary>
    public void Draw(int count, PrimitiveTopology topology)
    {
        if (CurrentShader is null)
            throw new InvalidOperationException(@"A shader must be bound before draw operations can begin.");

        if (CurrentVertexBuffer is null || CurrentIndexBuffer is null)
            throw new InvalidOperationException(@"A vertex buffer and index buffer must be bound before draw operations can begin.");

        Graphics.Draw(count, topology);
    }

    public void PushProjectionMatrix(Matrix4x4 matrix)
    {
        projMatrixStack.Push(matrix);
        setProjectionMatrix(matrix);
    }

    public void PopProjectionMatrix()
    {
        projMatrixStack.Pop();

        if (!projMatrixStack.TryPeek(out var matrix))
            matrix = Matrix4x4.Identity;

        setProjectionMatrix(matrix);
    }

    internal void BindShader(Shader shader)
    {
        shaderStack.Push(shader);
        setShader(shader);
    }

    internal void UnbindShader(Shader shader)
    {
        if (shaderStack.Count > 0 && shaderStack.Peek() != shader)
            throw new InvalidOperationException(@"Attempting to unbind shader whilst not currently bound.");

        shaderStack.Pop();

        if (shaderStack.TryPeek(out var next))
        {
            setShader(next);
        }
        else
        {
            setShader(null);
        }
    }

    internal void BindTexture(Texture texture, int unit)
    {
        if (prevBoundTextureUnit == unit && boundTextures[unit] == texture)
            return;

        Graphics.SetTexture(texture.Native, unit);
        boundTextures[unit] = texture;
        prevBoundTextureUnit = unit;
    }

    internal void UnbindTexture(Texture texture, int unit = 0)
    {
        if (boundTextures[unit] is null || boundTextures[unit] != texture)
            return;

        Graphics.SetTexture(null, unit);
        boundTextures[unit] = null!;
    }

    internal void BindIndexBuffer(Buffers.Buffer buffer, IndexFormat format)
    {
        if (prevBoundIndexFormat.HasValue && prevBoundIndexFormat.Value == format && CurrentIndexBuffer == buffer)
            return;

        Graphics.SetIndexBuffer(buffer.Native, format);
        CurrentIndexBuffer = buffer;
        prevBoundIndexFormat = format;
    }

    internal void BindVertexBuffer(Buffers.Buffer buffer, IVertexLayout layout)
    {
        if ((prevBoundVertexLayout?.Equals(layout) ?? false) && CurrentVertexBuffer == buffer)
            return;

        Graphics.SetVertexBuffer(buffer.Native, layout);
        CurrentVertexBuffer = buffer;
        prevBoundVertexLayout = layout;
    }

    internal void BindFrameBuffer(FrameBuffer? frameBuffer)
    {
        frameBufferStack.Push(frameBuffer);
        setFrameBuffer(frameBuffer);
    }

    internal void UnbindFrameBuffer(FrameBuffer? frameBuffer)
    {
        if (CurrentFrameBuffer != frameBuffer)
            throw new InvalidOperationException(@"Attempting to unbind framebuffer whilst currently not bound.");

        frameBufferStack.Pop();
        setFrameBuffer(frameBufferStack.Count > 0 ? frameBufferStack.Peek() : null);
    }

    internal void EnqueueDisposal(Action action)
    {
        disposals.Enqueue(action);
    }

    private void setDepth(DepthInfo depth)
    {
        if (CurrentDepth == depth)
            return;

        Graphics.SetDepth(depth);
        CurrentDepth = depth;
    }

    private void setViewport(Viewport viewport)
    {
        if (CurrentViewport == viewport)
            return;

        Graphics.SetViewport(viewport);
        CurrentViewport = viewport;
    }

    private void setScissor(Rectangle scissor)
    {
        if (CurrentScissor == scissor)
            return;

        Graphics.SetScissor(scissor);
        CurrentScissor = scissor;
    }

    private void setScissorState(bool enabled)
    {
        if (CurrentScissorState == enabled)
            return;

        Graphics.SetScissorState(enabled);
        CurrentScissorState = enabled;
    }

    private void setStencil(StencilInfo stencil)
    {
        if (CurrentStencil == stencil)
            return;

        Graphics.SetStencil(stencil);
        CurrentStencil = stencil;
    }

    private void setShader(Shader? shader)
    {
        if (CurrentShader == shader)
            return;

        Graphics.SetShader(shader?.Native);
        shader?.Native.Update();

        CurrentShader = shader;
    }

    private void setFrameBuffer(FrameBuffer? frameBuffer)
    {
        if (CurrentFrameBuffer == frameBuffer)
            return;

        Graphics.SetFrameBuffer(frameBuffer?.Native);
        CurrentFrameBuffer = frameBuffer;
    }

    private void setProjectionMatrix(Matrix4x4 matrix)
    {
        if (CurrentProjectionMatrix == matrix)
            return;

        CurrentProjectionMatrix = matrix;
        Uniforms.GetUniform<Matrix4x4>(GlobalUniforms.Projection).Value = matrix;
    }

    private struct BackBufferRenderTarget : IRenderTarget
    {
        public int Width => context.View.Size.Width;
        public int Height => context.View.Size.Height;
        private readonly GraphicsContext context;

        public BackBufferRenderTarget(GraphicsContext context)
        {
            this.context = context;
        }

        public void Bind()
        {
            context.BindFrameBuffer(null);
        }

        public void Unbind()
        {
            context.UnbindFrameBuffer(null);
        }

        public void Dispose()
        {
        }
    }
}
