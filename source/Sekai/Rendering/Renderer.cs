// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Numerics;
using Sekai.Graphics;
using Sekai.Mathematics;

namespace Sekai.Rendering;

public abstract class Renderer : FrameworkObject
{
    internal abstract void Render(GraphicsContext graphics);
}

public abstract class Renderer<T, U> : Renderer
    where T : Drawable
    where U : Camera
{
    private readonly List<U> cameras = new();
    private readonly List<T> drawables = new();
    private readonly IComparer<T> comparer;

    public Renderer()
    {
        comparer = CreateComparer();
    }

    internal sealed override void Render(GraphicsContext graphics)
    {
        var cameras = this.cameras.ToArray();
        var drawables = this.drawables.ToArray();

        var frontToBack = new List<T>();
        var backToFront = new List<T>();

        foreach (var drawable in drawables)
        {
            if (!drawable.Enabled || !drawable.HasStarted || drawable.Transform is null)
                continue;

            switch (drawable.SortMode)
            {
                case SortMode.BackToFront:
                    backToFront.Add(drawable);
                    break;

                default:
                    frontToBack.Add(drawable);
                    break;
            }
        }

        frontToBack.Sort(comparer);
        backToFront.Sort(comparer);
        backToFront.Reverse();

        foreach (var camera in cameras)
        {
            var target = camera.Target ?? graphics.BackBufferTarget;

            target.Bind();

            foreach (var drawable in frontToBack)
                renderDrawable(graphics, camera, drawable);

            foreach (var drawable in backToFront)
                renderDrawable(graphics, camera, drawable);

            target.Unbind();
        }
    }

    internal void Add(T drawable)
    {
        if (drawables.Contains(drawable))
            return;

        drawables.Add(drawable);
    }

    internal void Remove(T drawable)
    {
        drawables.Remove(drawable);
    }

    internal void Add(U camera)
    {
        if (cameras.Contains(camera))
            return;

        cameras.Add(camera);
    }

    internal void Remove(U camera)
    {
        cameras.Remove(camera);
    }

    private void renderDrawable(GraphicsContext graphics, U camera, T drawable)
    {
        if (IsCulled(camera, drawable))
            return;

        var matrix = camera.ProjMatrix * camera.ViewMatrix * (drawable.Transform?.WorldMatrix ?? Matrix4x4.Identity);
        graphics.PushProjectionMatrix(matrix);
        OnDrawStart(graphics);

        drawable.Draw(this);

        OnDrawEnd(graphics);
        graphics.PopProjectionMatrix();
    }

    /// <summary>
    /// Called before drawables begin drawing.
    /// </summary>
    protected virtual void OnDrawStart(GraphicsContext graphics)
    {
    }

    /// <summary>
    /// Called after drawables end drawing.
    /// </summary>
    protected virtual void OnDrawEnd(GraphicsContext graphics)
    {
    }

    /// <summary>
    /// Returns whether a given drawable is culled from rendering.
    /// </summary>
    /// <remarks>
    /// By default it checks whether a given drawable is in the same render group the camera is in and
    /// if the drawable has a non-empty bounding box, is inside the camera's frustum. Otherwise, it is
    /// never culled from rendering.
    /// </remarks>
    protected virtual bool IsCulled(U camera, T drawable)
    {
        if ((camera.Groups & drawable.Group) != 0)
            return true;

        if (drawable.Bounds != BoundingBox.Empty && drawable.Culling == CullingMode.Frustum)
        {
            var boundingBox = (BoundingBoxExt)drawable.Bounds;
            return !camera.Frustum.Contains(ref boundingBox);
        }

        return false;
    }

    /// <summary>
    /// Creates an <see cref="IComparer{T}"/> for depth-comparison.
    /// </summary>
    protected abstract IComparer<T> CreateComparer();
}
