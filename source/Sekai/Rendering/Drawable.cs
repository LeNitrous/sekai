// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Mathematics;
using Sekai.Scenes;

namespace Sekai.Rendering;

/// <summary>
/// Base class for all <see cref="Component"/>s capable of drawing to the scene.
/// </summary>
public abstract class Drawable : Script
{
    /// <summary>
    /// The drawable's sorting mode.
    /// </summary>
    public SortMode SortMode = SortMode.FrontToBack;

    /// <summary>
    /// The render groups where this drawable is visible to.
    /// </summary>
    public RenderGroup Group = RenderGroup.Default;

    /// <summary>
    /// The drawable's bounding box used when being culled from rendering.
    /// </summary>
    public BoundingBox Bounds = BoundingBox.Empty;

    /// <summary>
    /// The drawable's culling mode.
    /// </summary>
    public CullingMode Culling = CullingMode.Frustum;

    /// <summary>
    /// The drawable's transform obtained from the <see cref="Node"/> owning this drawable.
    /// </summary>
    public ITransform Transform => Owner is IRenderableNode owner
        ? owner.Transform
        : throw new InvalidOperationException(@"Component may not be attached or the owning node does not have a transform.");

    /// <summary>
    /// Performs drawing operations to the current scene.
    /// </summary>
    internal abstract void Draw(Renderer renderer);

    internal override bool CanAttach(Node node) => node is IRenderableNode;
}
