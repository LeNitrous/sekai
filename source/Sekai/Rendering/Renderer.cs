// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Graphics;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering;

public abstract class Renderer : FrameworkObject
{
    internal abstract void Render(GraphicsContext graphics);
}

public abstract class Renderer<TDrawable, TCamera> : Renderer
    where TDrawable : Drawable
    where TCamera : Camera
{
    private IRenderBatch? currentRenderBatch;
    private readonly List<TCamera> cameras = new();
    private readonly List<TDrawable> drawables = new();
    private readonly IComparer<TDrawable> comparer;
    private readonly Dictionary<Type, IRenderBatch> batches = new();

    public Renderer()
    {
        comparer = CreateComparer();
    }

    internal sealed override void Render(GraphicsContext graphics)
    {
        // This will be unsafe once we delve into multithreading!
        var cameras = CollectionsMarshal.AsSpan(this.cameras);
        var drawables = CollectionsMarshal.AsSpan(this.drawables);

        var frontToBack = new List<TDrawable>();
        var backToFront = new List<TDrawable>();

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

        backToFront.Sort(comparer);

        foreach (var camera in cameras)
        {
            var target = camera.Target ?? graphics.BackBufferTarget;

            target.Bind();

            if (frontToBack.Count > 0)
            {
                foreach (var drawable in frontToBack)
                    renderDrawable(graphics, camera, drawable);
            }

            if (backToFront.Count > 0)
            {
                for (int i = backToFront.Count - 1; i <= 0; i--)
                {
                    var drawable = backToFront[i];
                    renderDrawable(graphics, camera, drawable);
                }
            }

            target.Unbind();
        }
    }

    internal void Add(TDrawable drawable)
    {
        if (drawables.Contains(drawable))
            return;

        drawables.Add(drawable);
    }

    internal void Remove(TDrawable drawable)
    {
        drawables.Remove(drawable);
    }

    internal void Add(TCamera camera)
    {
        if (cameras.Contains(camera))
            return;

        cameras.Add(camera);
    }

    internal void Remove(TCamera camera)
    {
        cameras.Remove(camera);
    }

    private void renderDrawable(GraphicsContext graphics, TCamera camera, TDrawable drawable)
    {
        if (IsCulled(camera, drawable))
            return;

        var matrix = camera.ProjMatrix * camera.ViewMatrix * (drawable.Transform?.WorldMatrix ?? Matrix4x4.Identity);
        graphics.PushProjectionMatrix(matrix);

        drawable.Draw(this);

        ClearRenderBatch();
        graphics.PopProjectionMatrix();
    }

    /// <summary>
    /// Returns whether a given drawable is culled from rendering.
    /// </summary>
    /// <remarks>
    /// By default it checks whether a given drawable is in the same render group the camera is in and
    /// if the drawable has a non-empty bounding box, is inside the camera's frustum. Otherwise, it is
    /// never culled from rendering.
    /// </remarks>
    protected virtual bool IsCulled(TCamera camera, TDrawable drawable)
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
    protected abstract IComparer<TDrawable> CreateComparer();

    protected void AddRenderBatch<TPrimitive>(IRenderBatch<TPrimitive> batch)
        where TPrimitive : unmanaged, IPrimitive
    {
        if (batches.ContainsKey(typeof(TPrimitive)))
            throw new ArgumentException(@"A render batch with the given key has already been added", nameof(TPrimitive));

        batches.Add(typeof(TPrimitive), batch);
    }

    protected IRenderBatch<TPrimitive> GetRenderBatch<TPrimitive>()
        where TPrimitive : unmanaged, IPrimitive
    {
        if (!batches.TryGetValue(typeof(TPrimitive), out var batch))
            throw new Exception();

        if (currentRenderBatch != batch)
        {
            currentRenderBatch?.End();
            currentRenderBatch = batch;
            currentRenderBatch.Begin();
        }

        return (IRenderBatch<TPrimitive>)batch;
    }

    protected void ClearRenderBatch()
    {
        if (currentRenderBatch is null)
            return;

        currentRenderBatch.End();
        currentRenderBatch = null;
    }
}

