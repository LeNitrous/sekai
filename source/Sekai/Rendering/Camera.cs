// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Graphics.Textures;
using Sekai.Mathematics;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// The camera where all visible objects can be rendered to when visible.
/// </summary>
public abstract class Camera : Component
{
    /// <summary>
    /// The render groups this camera can render on to.
    /// </summary>
    public RenderGroup Groups { get; set; } = RenderGroup.Default;

    /// <summary>
    /// The camera's render target.
    /// </summary>
    /// <remarks>
    /// If null, the camera's contents will be drawn to the main back buffer.
    /// </remarks>
    public RenderTarget? Target { get; set; } = null;

    /// <summary>
    /// The camera's frustum used for culling objects.
    /// </summary>
    public BoundingFrustum Frustum { get; internal set; }

    /// <summary>
    /// The camera's view matrix.
    /// </summary>
    public Matrix4x4 ViewMatrix { get; internal set; }

    /// <summary>
    /// The camera's projection matrix.
    /// </summary>
    public Matrix4x4 ProjMatrix { get; internal set; }
}
