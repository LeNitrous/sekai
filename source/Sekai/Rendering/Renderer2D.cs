// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Textures;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering;

public class Renderer2D : Renderer<Drawable2D, Camera2D>
{
    private readonly GraphicsContext context = Services.Current.Resolve<GraphicsContext>();

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

    protected override IComparer<Drawable2D> CreateComparer() => Comparer<Drawable2D>.Create((x, y) => x.Transform.CompareTo(y.Transform));
}
