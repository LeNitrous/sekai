// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Mathematics;
using Sekai.Rendering;

namespace Sekai;

/// <summary>
/// A component variant that is capable of drawing to a <see cref="Renderer"/>.
/// </summary>
public abstract class Drawable : Component
{
    /// <summary>
    /// The sorting mode this drawable will use.
    /// </summary>
    public SortMode SortMode;

    /// <summary>
    /// The render groups this drawable is visible to.
    /// </summary>
    public RenderGroup Groups;

    /// <summary>
    /// The bounding box this drawable encompasses.
    /// </summary>
    public BoundingBox Bounds;

    /// <summary>
    /// The transform matrix for this drawable.
    /// </summary>
    internal Matrix Transform => Entity?.Transform?.WorldMatrix ?? Matrix.Identity;

    /// <summary>
    /// Called every frame on render if it is visible.
    /// </summary>
    public abstract void Draw();

    protected override void OnAttach()
    {
        base.OnAttach();
        // Game.Resolve<GameSystemCollection>().Get<Renderer>().Add(this);
    }

    protected override void OnDetach()
    {
        base.OnDetach();
        // Game.Resolve<GameSystemCollection>().Get<Renderer>().Remove(this);
    }
}
