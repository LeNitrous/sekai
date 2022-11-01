// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Drawing;
using Sekai.Framework.Windowing;

namespace Sekai.Framework.Graphics;

public interface IGraphicsContext : IDisposable
{
    /// <summary>
    /// Gets wither the graphics context has initialized.
    /// </summary>
    bool HasInitialized { get; }

    /// <summary>
    /// The current blending parameters.
    /// </summary>
    BlendingParameters CurrentBlend { get; }

    /// <summary>
    /// The current blending mask.
    /// </summary>
    BlendingMask CurrentBlendMask { get; }

    /// <summary>
    /// The current viewport.
    /// </summary>
    Rectangle CurrentViewport { get; }

    /// <summary>
    /// The current scissor rectangle.
    /// </summary>
    Rectangle CurrentScissor { get; }

    /// <summary>
    /// The current scissor state.
    /// </summary>
    bool CurrentScissorState { get; }

    /// <summary>
    /// The current clear info.
    /// </summary>
    ClearInfo CurrentClearInfo { get; }

    /// <summary>
    /// The current depth info.
    /// </summary>
    DepthInfo CurrentDepthInfo { get; }

    /// <summary>
    /// The current stencil info.
    /// </summary>
    StencilInfo CurrentStencilInfo { get; }

    /// <summary>
    /// Initializes the window to use this graphics context.
    /// </summary>
    void Initialize(IView view);

    /// <summary>
    /// Prepare for the next frame.
    /// </summary>
    void Prepare();

    /// <summary>
    /// Finish the current frame.
    /// </summary>
    void Finish();

    /// <summary>
    /// Presents the current frame to the window.
    /// </summary>
    void Present();

    /// <summary>
    /// Clears the current framebuffer.
    /// </summary>
    void Clear(ClearInfo info);

    /// <summary>
    /// Sets the current blending parameters.
    /// </summary>
    void SetBlend(BlendingParameters parameters);

    /// <summary>
    /// Sets the current blending mask.
    /// </summary>
    void SetBlendMask(BlendingMask mask);

    /// <summary>
    /// Applies new depth parameters.
    /// </summary>
    void PushDepth(DepthInfo info);

    /// <summary>
    /// Restores the previous depth parameters.
    /// </summary>
    void PopDepth();

    /// <summary>
    /// Applies a new viewport rectangle.
    /// </summary>
    void PushViewport(Rectangle viewport);

    /// <summary>
    /// Restores the previous viewport rectangle.
    /// </summary>
    void PopViewport();

    /// <summary>
    /// Applies a new scissor rectangle.
    /// </summary>
    void PushScissor(Rectangle scissor);

    /// <summary>
    /// Restores the previous scissor rectangle.
    /// </summary>
    void PopScissor();

    /// <summary>
    /// Applies a new scissor state.
    /// </summary>
    void PushScissorState(bool enabled);

    /// <summary>
    /// Restores the previous scissor state.
    /// </summary>
    void PopScissorState();

    /// <summary>
    /// Applies the new stencil parameters.
    /// </summary>
    void PushStencil(StencilInfo info);

    /// <summary>
    /// Restores the previous stencil parameters.
    /// </summary>
    void PopStencil();
}
