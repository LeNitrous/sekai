// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Mathematics;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Drawables are capable of drawing to the <see cref="Scene"/>.
/// </summary>
public abstract class Drawable : Scriptable
{
    /// <summary>
    /// The drawable's sorting mode.
    /// </summary>
    public SortMode SortMode { get; set; } = SortMode.FrontToBack;

    /// <summary>
    /// The render groups where this drawable is visible to.
    /// </summary>
    public RenderGroup Group { get; set; } = RenderGroup.Default;

    /// <summary>
    /// The drawable's bounding box used when <see cref="Culling"/> is <see cref="CullingMode.Frustum"/>.
    /// </summary>
    public BoundingBox Bounds { get; set; } = BoundingBox.Empty;

    /// <summary>
    /// The culling method used on this drawable.
    /// </summary>
    public CullingMode Culling { get; set; } = CullingMode.Frustum;

    /// <summary>
    /// Performs draw operations to the render target.
    /// </summary>
    public abstract void Draw(RenderContext context);

    /// <summary>
    /// The kind of scene this drawable is allowed to be part of.
    /// </summary>
    internal abstract SceneKind Kind { get; }

    /// <summary>
    /// Gets this drawable's transform.
    /// </summary>
    /// <returns>The drawable's ransform.</returns>
    internal abstract Transform GetTransform();
}
