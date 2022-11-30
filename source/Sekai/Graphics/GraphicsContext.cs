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
    public Rectangle CurrentViewport { get; private set; }

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
    /// The current view matrix.
    /// </summary>
    public Matrix4x4 CurrentViewMatrix { get; private set; }

    /// <summary>
    /// The current projection matrix.
    /// </summary>
    public Matrix4x4 CurrentProjectionMatrix  { get; private set; }

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
    private readonly Stack<Rectangle> viewportStack = new();
    private readonly Stack<Rectangle> scissorStack = new();
    private readonly Stack<bool> scissorStateStack = new();
    private readonly Stack<Shader> shaderStack = new();
    private readonly Stack<FrameBuffer> frameBufferStack = new();
    private readonly Stack<Matrix4x4> projMatrixStack = new();
    private readonly Stack<Matrix4x4> viewMatrixStack = new();
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
        CurrentViewport = Rectangle.Empty;
        CurrentViewMatrix = Matrix4x4.Identity;
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
        viewMatrixStack.Clear();
        scissorStateStack.Clear();
        boundTextures.AsSpan().Clear();

        Graphics.Reset();

        PushScissorState(true);
        PushViewport(new Rectangle(0, 0, View.Size.Width, View.Size.Height));
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
    public void PushViewport(Rectangle viewport)
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
        setProjectionMatrix(projMatrixStack.Peek());
    }

    public void PushViewMatrix(Matrix4x4 matrix)
    {
        viewMatrixStack.Push(matrix);
        setViewMatrix(matrix);
    }

    public void PopViewMatrix()
    {
        viewMatrixStack.Pop();
        setViewMatrix(viewMatrixStack.Peek());
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
        setShader(shaderStack.Peek());
    }

    internal void BindTexture(Texture texture, int unit)
    {
        if (prevBoundTextureUnit == unit && boundTextures[unit] == texture)
            return;

        Graphics.SetTexture(texture.Native, unit);
        boundTextures[unit] = texture;
        prevBoundTextureUnit = unit;
    }

    internal void UnbindTexture(int unit = 0)
    {
        if (boundTextures[unit] is null)
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

    internal void BindFrameBuffer(FrameBuffer frameBuffer)
    {
        frameBufferStack.Push(frameBuffer);
        setFrameBuffer(frameBuffer);
    }

    internal void UnbindFrameBuffer(FrameBuffer frameBuffer)
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

    private void setViewport(Rectangle viewport)
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

    private void setShader(Shader shader)
    {
        if (CurrentShader == shader)
            return;

        Graphics.SetShader(shader.Native);
        shader.Native.Update();

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

    private void setViewMatrix(Matrix4x4 matrix)
    {
        if (CurrentViewMatrix == matrix)
            return;

        CurrentViewMatrix = matrix;
        Uniforms.GetUniform<Matrix4x4>(GlobalUniforms.View).Value = matrix;
    }
}
