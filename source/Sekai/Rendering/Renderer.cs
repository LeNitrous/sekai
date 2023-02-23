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

internal sealed class Renderer : DisposableObject
{
    private readonly GraphicsContext graphics;
    private readonly RenderBatchManager batches;

    private RenderKind allowedCollectionKind;
    private RendererPhase phase;
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
    public void Begin(RenderKind kind)
    {
        if (phase != RendererPhase.Idle)
            throw new InvalidOperationException(@"The renderer is not ready to begin a new context.");

        phase = RendererPhase.Collect;

        allowedCollectionKind = kind;
    }

    /// <summary>
    /// Ends the collection phase.
    /// </summary>
    public void End()
    {
        if (phase != RendererPhase.Collect)
            throw new InvalidOperationException(@"The renderer cannot end the current phase at this state.");

        phase = RendererPhase.Ready;

        allowedCollectionKind = RenderKind.Unknown;
    }

    /// <summary>
    /// Collects a <see cref="IRenderObject"/> object.
    /// </summary>
    /// <param name="renderObject">The render object.</param>
    public void Collect(IRenderObject renderObject)
    {
        if (phase != RendererPhase.Collect)
            throw new InvalidOperationException(@"The renderer is not ready for collecting scene objects.");

        if (renderObject.Kind != allowedCollectionKind)
            throw new ArgumentException("This object cannot be collected by the renderer at this state.", nameof(renderObject));

        switch (renderObject)
        {
            case Camera camera:
                cameras.Add(camera);
                break;

            case Drawable drawable:
                drawables.Add(drawable);
                break;
        }
    }

    /// <summary>
    /// Flushes the collected collectibles.
    /// </summary>
    public void Flush()
    {
        if (phase != RendererPhase.Ready)
            throw new InvalidOperationException(@"The renderer is not ready to flush scene objects.");

        foreach (var camera in CollectionsMarshal.AsSpan(cameras))
        {
            var visible = drawables.Where(d => !isCulled(camera, d));
            frontToBack.AddRange(visible.Where(static d => d.SortMode == SortMode.FrontToBack));
            backToFront.AddRange(visible.Where(static d => d.SortMode == SortMode.BackToFront));

            using (graphics.BeginContext())
            {
                var target = camera.Target ?? graphics.DefaultRenderTarget;

                graphics.SetRenderTarget(target);
                graphics.Viewport = new(0, 0, target.Width, target.Height, 0, 1);

                if (frontToBack.Count > 0)
                {
                    frontToBack.Sort(new DrawableComparer(camera, false));

                    foreach (var drawable in CollectionsMarshal.AsSpan(frontToBack))
                        renderDrawable(camera, drawable);
                }

                if (backToFront.Count > 0)
                {
                    backToFront.Sort(new DrawableComparer(camera, true));

                    foreach (var drawable in CollectionsMarshal.AsSpan(backToFront))
                        renderDrawable(camera, drawable);
                }
            }
        }

        cameras.Clear();
        drawables.Clear();
        frontToBack.Clear();
        backToFront.Clear();

        phase = RendererPhase.Idle;
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

    private enum RendererPhase
    {
        /// <summary>
        /// Idle phase.
        /// </summary>
        Idle = 0,

        /// <summary>
        /// Collection phase.
        /// </summary>
        Collect,

        /// <summary>
        /// Ready phase.
        /// </summary>
        Ready
    }
}
