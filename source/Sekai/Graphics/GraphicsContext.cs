// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
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

    private readonly IView view;
    private readonly IGraphicsSystem graphics;
    private readonly Stack<StencilInfo> stencilStack = new();
    private readonly Stack<DepthInfo> depthStack = new();
    private readonly Stack<Rectangle> viewportStack = new();
    private readonly Stack<Rectangle> scissorStack = new();
    private readonly Stack<bool> scissorStateStack = new();
    private readonly Stack<Shader> shaderStack = new();
    private readonly Texture[] boundTextures = new Texture[16];
    private int prevBoundTextureUnit = -1;
    private IndexFormat? prevBoundIndexFormat;
    private IVertexLayout? prevBoundVertexLayout;

    internal GraphicsContext(IGraphicsSystem graphics, IView view)
    {
        this.view = view;
        this.graphics = graphics;
    }

    /// <summary>
    /// Prepare for the next frame.
    /// </summary>
    public void Prepare()
    {
        Reset();
        Clear(new ClearInfo(new Color4(0, 0, 0, 1f)));
    }

    /// <summary>
    /// Presents the current frame to the window.
    /// </summary>
    public void Present()
    {
        graphics.Present();
    }

    /// <summary>
    /// Resets the state of the graphics context.
    /// </summary>
    public void Reset()
    {
        CurrentBlend = new();
        CurrentScissor = Rectangle.Empty;
        CurrentViewport = Rectangle.Empty;
        CurrentScissorState = false;
        prevBoundTextureUnit = -1;
        prevBoundIndexFormat = null;
        prevBoundVertexLayout = null;

        depthStack.Clear();
        stencilStack.Clear();
        scissorStack.Clear();
        viewportStack.Clear();
        scissorStateStack.Clear();
        boundTextures.AsSpan().Clear();

        graphics.Reset();

        PushScissorState(true);
        PushViewport(new Rectangle(0, 0, view.Size.Width, view.Size.Height));
        PushScissor(new Rectangle(0, 0, view.Size.Width, view.Size.Height));
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

        graphics.Clear(info);

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

        graphics.SetBlend(blend);
        CurrentBlend = blend;
    }

    /// <summary>
    /// Sets the current blending mask.
    /// </summary>
    public void SetBlendMask(BlendingMask mask)
    {
        if (CurrentBlendMask == mask)
            return;

        graphics.SetBlendMask(mask);
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
        graphics.Draw(count, topology);
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

        graphics.SetTexture(texture.Native, unit);
        boundTextures[unit] = texture;
        prevBoundTextureUnit = unit;
    }

    internal void UnbindTexture(int unit = 0)
    {
        if (boundTextures[unit] is null)
            return;

        graphics.SetTexture(null, unit);
        boundTextures[unit] = null!;
    }

    internal void BindIndexBuffer(Buffers.Buffer buffer, IndexFormat format)
    {
        if (prevBoundIndexFormat.HasValue && prevBoundIndexFormat.Value == format && CurrentIndexBuffer == buffer)
            return;

        graphics.SetIndexBuffer(buffer.Native, format);
        CurrentIndexBuffer = buffer;
        prevBoundIndexFormat = format;
    }

    internal void BindVertexBuffer(Buffers.Buffer buffer, IVertexLayout layout)
    {
        if ((prevBoundVertexLayout?.Equals(layout) ?? false) && CurrentVertexBuffer == buffer)
            return;

        graphics.SetVertexBuffer(buffer.Native, layout);
        CurrentVertexBuffer = buffer;
        prevBoundVertexLayout = layout;
    }

    private void setDepth(DepthInfo depth)
    {
        if (CurrentDepth == depth)
            return;

        graphics.SetDepth(depth);
        CurrentDepth = depth;
    }

    private void setViewport(Rectangle viewport)
    {
        if (CurrentViewport == viewport)
            return;

        graphics.SetViewport(viewport);
        CurrentViewport = viewport;
    }

    private void setScissor(Rectangle scissor)
    {
        if (CurrentScissor == scissor)
            return;

        graphics.SetScissor(scissor);
        CurrentScissor = scissor;
    }

    private void setScissorState(bool enabled)
    {
        if (CurrentScissorState == enabled)
            return;

        graphics.SetScissorState(enabled);
        CurrentScissorState = enabled;
    }

    private void setStencil(StencilInfo stencil)
    {
        if (CurrentStencil == stencil)
            return;

        graphics.SetStencil(stencil);
        CurrentStencil = stencil;
    }

    private void setShader(Shader shader)
    {
        if (CurrentShader == shader)
            return;

        graphics.SetShader(shader.Native);
        CurrentShader = shader;
    }
}
