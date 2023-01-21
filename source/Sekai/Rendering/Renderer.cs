// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;

namespace Sekai.Rendering;

public sealed class Renderer : DependencyObject
{
    [Resolved]
    private GraphicsContext graphics { get; set; } = null!;

    private RenderBatchManager? batches;

    private bool canFlush;
    private bool canCollect;
    private readonly List<ICamera> cameras = new();
    private readonly List<IDrawable> drawables = new();
    private readonly List<IDrawable> frontToBack = new();
    private readonly List<IDrawable> backToFront = new();

    /// <summary>
    /// Begins collection phase.
    /// </summary>
    public void Begin()
    {
        if (canCollect || canFlush)
            return;

        batches ??= new();

        cameras.Clear();
        drawables.Clear();
        frontToBack.Clear();
        backToFront.Clear();

        canCollect = true;
    }

    /// <summary>
    /// Ends collection phase.
    /// </summary>
    public void End()
    {
        if (!canCollect || canFlush)
            return;

        canCollect = false;
        canFlush = true;
    }

    /// <summary>
    /// Collects a camera for rendering.
    /// </summary>
    public void Collect(ICamera camera)
    {
        ensureCanCollect();
        cameras.Add(camera);
    }

    /// <summary>
    /// Collects a drawable for rendering.
    /// </summary>
    public void Collect(IDrawable drawable)
    {
        ensureCanCollect();
        drawables.Add(drawable);
    }

    private void ensureCanCollect()
    {
        if (!canCollect)
            throw new InvalidOperationException(@"Cannot collect renderer objects at this current state.");
    }

    /// <summary>
    /// Flushes the collected renderer objects and renders them to the specified targets.
    /// </summary>
    public void Render()
    {
        if (!canFlush)
            throw new InvalidOperationException(@"Cannot flush at this current state.");

        foreach (var camera in CollectionsMarshal.AsSpan(cameras))
        {
            var visible = drawables.Where(d => !isCulled(camera, d));
            frontToBack.AddRange(visible.Where(d => d.SortMode == SortMode.FrontToBack));
            backToFront.AddRange(visible.Where(d => d.SortMode == SortMode.BackToFront));

            var target = camera.Target ?? graphics.BackBufferTarget;

            if (frontToBack.Count > 0)
            {
                var comparer = new DrawableComparer(camera, false);
                frontToBack.Sort(comparer);

                foreach (var drawable in CollectionsMarshal.AsSpan(frontToBack))
                    renderDrawable(camera, drawable);
            }

            if (backToFront.Count > 0)
            {
                var comparer = new DrawableComparer(camera, true);
                backToFront.Sort(comparer);

                foreach (var drawable in CollectionsMarshal.AsSpan(backToFront))
                    renderDrawable(camera, drawable);
            }
        }

        canFlush = false;
    }

    private void renderDrawable(ICamera camera, IDrawable drawable)
    {
        using (graphics.BeginContext())
        {
            graphics.SetTransform(drawable.Transform.WorldMatrix, camera.ViewMatrix, camera.ProjMatrix);
            drawable.Draw(new(camera, graphics, batches!));
            batches!.EndCurrentBatch();
        }
    }

    private static bool isCulled(ICamera camera, IDrawable drawable)
    {
        if ((camera.Groups & drawable.Group) != 0)
            return true;

        if (drawable.Bounds != BoundingBox.Empty && drawable.Culling == CullingMode.Frustum)
        {
            var bounds = (BoundingBoxExt)drawable.Bounds;
            return !camera.Frustum.Contains(ref bounds);
        }

        return false;
    }

    protected override void Destroy() => batches?.Dispose();

    private class DrawableComparer : Comparer<IDrawable>
    {
        private readonly bool invert;
        private readonly ICamera camera;

        public DrawableComparer(ICamera camera, bool invert)
        {
            this.invert = invert;
            this.camera = camera;
        }

        public override int Compare(IDrawable? x, IDrawable? y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            float distanceX = Vector3.Distance(x.Transform.WorldMatrix.Translation, camera.Transform.WorldMatrix.Translation);
            float distanceY = Vector3.Distance(y.Transform.WorldMatrix.Translation, camera.Transform.WorldMatrix.Translation);

            if (invert)
            {
                distanceX = -distanceX;
                distanceY = -distanceY;
            }

            return distanceX.CompareTo(distanceY);
        }
    }
}
