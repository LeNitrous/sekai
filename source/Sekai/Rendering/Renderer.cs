// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Sekai.Assets;
using Sekai.Graphics;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;
using Sekai.Scenes;

namespace Sekai.Rendering;

public sealed class Renderer : DisposableObject
{
    private readonly GraphicsContext graphics;
    private readonly RenderBatchManager batches;

    private bool canFlush;
    private SceneKind? kind;
    private readonly List<Camera> cameras = new();
    private readonly List<Drawable> drawables = new();
    private readonly List<Drawable> frontToBack = new();
    private readonly List<Drawable> backToFront = new();

    public Renderer(GraphicsContext graphics, AssetLoader assets)
    {
        batches = new(graphics, assets);
        this.graphics = graphics;
    }

    /// <summary>
    /// Begins collection phase.
    /// </summary>
    public void Begin(SceneKind kind)
    {
        if (this.kind.HasValue || canFlush)
            return;

        this.kind = kind;

        cameras.Clear();
        drawables.Clear();
        frontToBack.Clear();
        backToFront.Clear();
    }

    /// <summary>
    /// Ends collection phase.
    /// </summary>
    public void End()
    {
        if (!kind.HasValue || canFlush)
            return;

        canFlush = true;
        kind = null;
    }

    /// <summary>
    /// Collects a camera for rendering.
    /// </summary>
    public void Collect(Camera camera)
    {
        if (!kind.HasValue)
            throw new InvalidOperationException(@"Cannot collect renderer objects at this current state.");

        if (camera.Kind != kind.Value)
            throw new ArgumentException("Camera cannot be collected by this renderer.");

        cameras.Add(camera);
    }

    /// <summary>
    /// Collects a drawable for rendering.
    /// </summary>
    public void Collect(Drawable drawable)
    {
        if (!kind.HasValue)
            throw new InvalidOperationException(@"Cannot collect renderer objects at this current state.");

        if (drawable.Kind != kind.Value)
            throw new ArgumentException("Drawable cannot be collected by this renderer.");

        drawables.Add(drawable);
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

            var target = camera.Target ?? graphics.DefaultRenderTarget;

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

    private void renderDrawable(Camera camera, Drawable drawable)
    {
        using (graphics.BeginContext())
        {
            graphics.SetTransform(drawable.GetTransform().WorldMatrix, camera.ViewMatrix, camera.ProjMatrix);
            drawable.Draw(new(camera, graphics, batches!));
            batches!.EndCurrentBatch();
        }
    }

    private static bool isCulled(Camera camera, Drawable drawable)
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

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            batches.Dispose();
    }

    private class DrawableComparer : Comparer<Drawable>
    {
        private readonly bool invert;
        private readonly Camera camera;

        public DrawableComparer(Camera camera, bool invert)
        {
            this.invert = invert;
            this.camera = camera;
        }

        public override int Compare(Drawable? x, Drawable? y)
        {
            if (ReferenceEquals(x, y))
                return 0;

            if (x is null)
                return -1;

            if (y is null)
                return 1;

            float distanceX = Vector3.Distance(x.GetTransform().WorldMatrix.Translation, camera.GetTransform().WorldMatrix.Translation);
            float distanceY = Vector3.Distance(y.GetTransform().WorldMatrix.Translation, camera.GetTransform().WorldMatrix.Translation);

            if (invert)
            {
                distanceX = -distanceX;
                distanceY = -distanceY;
            }

            return distanceX.CompareTo(distanceY);
        }
    }
}
