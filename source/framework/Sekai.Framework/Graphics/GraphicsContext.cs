// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Drawing;
using Sekai.Framework.Windowing;

namespace Sekai.Framework.Graphics;

public abstract class GraphicsContext : FrameworkObject, IGraphicsContext
{
    public bool HasInitialized { get; private set; }
    public BlendingParameters CurrentBlend { get; private set; }
    public BlendingMask CurrentBlendMask { get; private set; }
    public Rectangle CurrentViewport { get; private set; }
    public Rectangle CurrentScissor { get; private set; }
    public bool CurrentScissorState { get; private set; }
    public ClearInfo CurrentClearInfo { get; private set; }
    public DepthInfo CurrentDepthInfo { get; private set; }
    public StencilInfo CurrentStencilInfo { get; private set; }

    private readonly Stack<StencilInfo> stencilInfoStack = new();
    private readonly Stack<DepthInfo> depthInfoStack = new();
    private readonly Stack<Rectangle> viewportStack = new();
    private readonly Stack<Rectangle> scissorStack = new();
    private readonly Stack<bool> scissorStateStack = new();
    private IView view = null!;
    private bool hasPrepared;

    public void Initialize(IView view)
    {
        if (HasInitialized)
            return;

        this.view = view;

        InitializeImpl(view);

        HasInitialized = true;
    }

    public void Present()
    {
        ensureHasInitialized();
        PresentImpl();
    }

    public void Prepare()
    {
        ensureHasInitialized();

        if (hasPrepared)
            throw new InvalidOperationException($"{nameof(Finish)} must be called before starting another {nameof(Prepare)}.");

        CurrentBlend = new();

        viewportStack.Clear();
        scissorStack.Clear();
        scissorStateStack.Clear();
        depthInfoStack.Clear();
        stencilInfoStack.Clear();

        CurrentScissor = Rectangle.Empty;
        CurrentViewport = Rectangle.Empty;

        PrepareImpl();

        PushScissorState(true);
        PushViewport(new Rectangle(0, 0, view.Size.Width, view.Size.Height));
        PushScissor(new Rectangle(0, 0, view.Size.Width, view.Size.Height));
        PushDepth(new DepthInfo(true));
        PushStencil(new StencilInfo(false));

        Clear(new ClearInfo(new Color4(0, 0, 0, 1f)));

        hasPrepared = true;
    }

    public void Finish()
    {
        ensureHasInitialized();

        if (!hasPrepared)
            throw new InvalidOperationException($"{nameof(Prepare)} must be called first before {nameof(Finish)}.");

        FinishImpl();

        hasPrepared = false;
    }

    public void Clear(ClearInfo info)
    {
        PushDepth(new DepthInfo(writeDepth: true));
        PushScissorState(false);

        ClearImpl(info);

        CurrentClearInfo = info;

        PopScissorState();
        PopDepth();
    }

    public void SetBlend(BlendingParameters parameters)
    {
        if (CurrentBlend == parameters)
            return;

        SetBlendImpl(parameters);

        CurrentBlend = parameters;
    }

    public void SetBlendMask(BlendingMask mask)
    {
        if (CurrentBlendMask == mask)
            return;

        SetBlendMaskImpl(mask);

        CurrentBlendMask = mask;
    }

    public void PushDepth(DepthInfo info)
    {
        depthInfoStack.Push(info);
        setDepth(info);
    }

    public void PopDepth()
    {
        depthInfoStack.Pop();
        setDepth(depthInfoStack.Peek());
    }

    public void PushViewport(Rectangle viewport)
    {
        viewportStack.Push(viewport);
        setViewport(viewport);
    }

    public void PopViewport()
    {
        viewportStack.Pop();
        setViewport(viewportStack.Peek());
    }

    public void PushScissor(Rectangle scissor)
    {
        scissorStack.Push(scissor);
        setScissor(scissor);
    }

    public void PopScissor()
    {
        scissorStack.Pop();
        setScissor(scissorStack.Peek());
    }

    public void PushScissorState(bool enabled)
    {
        scissorStateStack.Push(enabled);
        setScissorState(enabled);
    }

    public void PopScissorState()
    {
        scissorStateStack.Pop();
        setScissorState(scissorStateStack.Peek());
    }

    public void PushStencil(StencilInfo info)
    {
        stencilInfoStack.Push(info);
        setStencil(info);
    }

    public void PopStencil()
    {
        stencilInfoStack.Pop();
        setStencil(stencilInfoStack.Peek());
    }

    private void setDepth(DepthInfo info)
    {
        ensureHasInitialized();

        if (CurrentDepthInfo == info)
            return;

        SetDepth(info);

        CurrentDepthInfo = info;
    }

    private void setScissor(Rectangle scissor)
    {
        ensureHasInitialized();

        if (CurrentScissor == scissor)
            return;

        SetScissor(scissor);

        CurrentScissor = scissor;
    }

    private void setStencil(StencilInfo info)
    {
        ensureHasInitialized();

        if (CurrentStencilInfo == info)
            return;

        SetStencil(info);

        CurrentStencilInfo = info;
    }

    private void setScissorState(bool state)
    {
        ensureHasInitialized();

        if (CurrentScissorState == state)
            return;

        SetScissorState(state);

        CurrentScissorState = state;
    }

    private void setViewport(Rectangle viewport)
    {
        ensureHasInitialized();

        if (CurrentViewport == viewport)
            return;

        SetViewport(viewport);

        CurrentViewport = viewport;
    }

    private void ensureHasInitialized()
    {
        if (!HasInitialized)
            throw new InvalidOperationException("The graphics context needs to be initialized.");
    }

    protected abstract void InitializeImpl(IView view);

    protected abstract void PrepareImpl();

    protected abstract void PresentImpl();

    protected abstract void FinishImpl();

    protected abstract void ClearImpl(ClearInfo info);

    protected abstract void SetDepth(DepthInfo info);

    protected abstract void SetStencil(StencilInfo info);

    protected abstract void SetViewport(Rectangle viewport);

    protected abstract void SetScissor(Rectangle scissor);

    protected abstract void SetScissorState(bool state);

    protected abstract void SetBlendImpl(BlendingParameters parameters);

    protected abstract void SetBlendMaskImpl(BlendingMask mask);
}
