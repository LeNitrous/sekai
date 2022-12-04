// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Graphics.Textures;
using Sekai.Mathematics;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// The scene's camera where all visible objects can be rendered to when visible.
/// </summary>
public abstract class Camera : Component
{
    /// <summary>
    /// The render groups where this camera is visisble to.
    /// </summary>
    public RenderGroup Groups = RenderGroup.Default;

    /// <summary>
    /// The camera's render target.
    /// </summary>
    /// <remarks>
    /// If null, everything rendered in this camera is drawn to the backbuffer.
    /// </remarks>
    public RenderTarget? Target = null;

    /// <summary>
    /// The camera's view matrix.
    /// </summary>
    public Matrix4x4 ViewMatrix { get; internal set; }

    /// <summary>
    /// The camera's projection matrix.
    /// </summary>
    public Matrix4x4 ProjMatrix { get; internal set; }

    /// <summary>
    /// The camera's view matrix inversed.
    /// </summary>
    public Matrix4x4 ViewMatrixInverse { get; internal set; }

    /// <summary>
    /// THe caerma's projection matrix inversed.
    /// </summary>
    public Matrix4x4 ProjMatrixInverse { get; internal set; }

    /// <summary>
    /// The camera's bounding frustum used for culling objects.
    /// </summary>
    public BoundingFrustum Frustum { get; internal set; }
}
