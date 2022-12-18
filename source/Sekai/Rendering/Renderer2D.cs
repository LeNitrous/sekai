// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Collections.Generic;
using System.Numerics;
using Sekai.Allocation;
using Sekai.Graphics;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering;

public class Renderer2D : Renderer<Drawable2D, Camera2D>
{
    private Texture? currentTexture;
    private readonly GraphicsContext context = Services.Current.Resolve<GraphicsContext>();

    public Renderer2D()
    {
        AddBatch<Line2D>(new LineBatch2D<ColoredVertex2D>(1000));
        AddBatch<Quad2D>(new QuadBatch2D<TexturedVertex2D>(1000));
    }

    public void Draw(Line2D line)
    {
        Draw(line, Color4.White);
    }

    public void Draw(Line2D line, Color4 color)
    {
        var batch = GetBatch<Line2D, ColoredVertex2D>();

        batch.Add(new ColoredVertex2D
        {
            Color = color,
            Position = line.Start,
        });

        batch.Add(new ColoredVertex2D
        {
            Color = color,
            Position = line.End,
        });
    }

    public void Draw(Quad2D quad)
    {
        Draw(quad, context.WhitePixel, Rectangle.Empty, Color4.White);
    }

    public void Draw(Quad2D quad, Color4 color)
    {
        Draw(quad, context.WhitePixel, Rectangle.Empty, color);
    }

    public void Draw(Quad2D quad, Texture texture, Rectangle textureRect)
    {
        Draw(quad, texture, textureRect, Color4.White);
    }

    public void Draw(Quad2D quad, Texture texture, Rectangle textureRect, Color4 color)
    {
        var batch = GetBatch<Quad2D, TexturedVertex2D>();

        if (currentTexture != texture)
        {
            batch.Flush();
            currentTexture?.Unbind();
            currentTexture = texture;
            currentTexture.Bind();
        }

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.TopLeft,
            TexCoord = new Vector2(textureRect.Left, textureRect.Top),
        });

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.BottomLeft,
            TexCoord = new Vector2(textureRect.Left, textureRect.Bottom),
        });

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.BottomRight,
            TexCoord = new Vector2(textureRect.Right, textureRect.Bottom),
        });

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.TopRight,
            TexCoord = new Vector2(textureRect.Right, textureRect.Top),
        });
    }

    protected override void ClearCurrentBatch()
    {
        base.ClearCurrentBatch();
        currentTexture?.Unbind();
        currentTexture = null;
    }

    protected override IComparer<Drawable2D> CreateComparer() => Comparer<Drawable2D>.Create((x, y) => x.Transform.CompareTo(y.Transform));
}
