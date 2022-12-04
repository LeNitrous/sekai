// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Numerics;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Textures;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering;

public class Renderer2D : Renderer<Drawable2D, Camera2D>
{
    private IRenderBatch? currentRenderBatch;
    private readonly GraphicsContext context = Services.Current.Resolve<GraphicsContext>();
    private readonly Dictionary<Type, IRenderBatch> batches = new();

    public Renderer2D()
    {
        AddRenderBatch(new QuadBatch(1000));
        AddRenderBatch(new LineBatch2D(1000));
    }

    public void Draw(Line2D line)
    {
        Draw(line, null);
    }

    public void Draw(Line2D line, Color4? color = null)
    {
        var batch = (LineBatch2D)GetRenderBatch<Line2D>();
        batch.Color = color ?? Color4.White;
        batch.Collect(line);
    }

    public void Draw(Quad quad)
    {
        Draw(quad, null, null);
    }

    public void Draw(Quad quad, Color4 color)
    {
        Draw(quad, null, color);
    }

    public void Draw(Quad quad, Texture texture)
    {
        Draw(quad, texture, null);
    }

    public void Draw(Quad quad, Texture? texture = null, Color4? color = null)
    {
        var batch = (QuadBatch)GetRenderBatch<Quad>();
        batch.Color = color ?? Color4.White;
        batch.Texture = texture ?? context.WhitePixel;
        batch.Collect(quad);
    }

    protected void AddRenderBatch<T>(IRenderBatch2D<T> batch)
        where T : unmanaged, IPrimitive<Vector2>
    {
        if (batches.ContainsKey(typeof(T)))
            return;

        batches.Add(typeof(T), batch);
    }

    protected IRenderBatch2D<T> GetRenderBatch<T>()
        where T : unmanaged, IPrimitive<Vector2>
    {
        if (!batches.TryGetValue(typeof(T), out var batch))
            throw new Exception();

        if (currentRenderBatch != batch)
        {
            currentRenderBatch?.End();
            currentRenderBatch = batch;
            currentRenderBatch.Begin();
        }

        return (IRenderBatch2D<T>)batch;
    }

    protected void ClearRenderBatch()
    {
        if (currentRenderBatch is null)
            return;

        currentRenderBatch.End();
        currentRenderBatch = null;
    }

    protected override void OnDrawEnd(GraphicsContext graphics)
    {
        ClearRenderBatch();
    }

    protected override IComparer<Drawable2D> CreateComparer() => Comparer<Drawable2D>.Create((x, y) => x.Transform.CompareTo(y.Transform));
}
