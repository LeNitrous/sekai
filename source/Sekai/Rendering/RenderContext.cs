// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Numerics;
using Sekai.Graphics;
using Sekai.Graphics.Textures;
using Sekai.Graphics.Vertices;
using Sekai.Mathematics;
using Sekai.Rendering.Batches;
using Sekai.Rendering.Primitives;

namespace Sekai.Rendering;

public class RenderContext
{
    /// <summary>
    /// The current camera.
    /// </summary>
    public readonly ICamera Camera;

    /// <summary>
    /// The current graphics context.
    /// </summary>
    public readonly GraphicsContext Graphics;

    private Texture? currentTexture;
    private readonly RenderBatchManager batches;

    internal RenderContext(ICamera camera, GraphicsContext graphics, RenderBatchManager batches)
    {
        Camera = camera;
        Graphics = graphics;
        this.batches = batches;
    }

    /// <inheritdoc cref="Draw(Line3D, Color4)"/>
    public void Draw(Line3D line)
    {
        Draw(line, Color4.White);
    }

    /// <summary>
    /// Draws a line in three-dimensional space.
    /// </summary>
    /// <param name="line">The line to draw.</param>
    /// <param name="color">The line color.</param>
    public void Draw(Line3D line, Color4 color)
    {
        batches.GetBatch<Line3D, ColoredVertex3D>(out var batch);

        batch.Add(new ColoredVertex3D
        {
            Color = color,
            Position = line.Start
        });

        batch.Add(new ColoredVertex3D
        {
            Color = color,
            Position = line.End
        });
    }

    /// <inheritdoc cref="Draw(Line2D, Color4)"/>
    public void Draw(Line2D line)
    {
        Draw(line, Color4.White);
    }

    public void Draw(Line2D line, Color4 color)
    {
        batches.GetBatch<Line2D, ColoredVertex2D>(out var batch);

        batch.Add(new ColoredVertex2D
        {
            Color = color,
            Position = line.Start
        });

        batch.Add(new ColoredVertex2D
        {
            Color = color,
            Position = line.End
        });
    }

    /// <inheritdoc cref="Draw(Quad, Texture, RectangleF, Color4)"/>
    public void Draw(Quad quad)
    {
        Draw(quad, Color.White);
    }

    /// <inheritdoc cref="Draw(Quad, Texture, RectangleF, Color4)"/>
    public void Draw(Quad quad, Color4 color)
    {
        Draw(quad, Graphics.WhitePixel, new RectangleF(0, 0, 1, 1), color);
    }

    /// <inheritdoc cref="Draw(Quad, Texture, RectangleF, Color4)"/>
    public void Draw(Quad quad, Texture texture)
    {
        Draw(quad, texture, new RectangleF(0, 0, 1, 1), Color4.White);
    }

    /// <inheritdoc cref="Draw(Quad, Texture, RectangleF, Color4)"/>
    public void Draw(Quad quad, Texture texture, RectangleF textureRect)
    {
        Draw(quad, texture, textureRect, Color4.White);
    }

    /// <summary>
    /// Draws a textured quad.
    /// </summary>
    /// <param name="quad">The quad to draw.</param>
    /// <param name="texture">The texture to use.</param>
    /// <param name="textureRect">The texture region to use.</param>
    /// <param name="color">The quad's color.</param>
    public void Draw(Quad quad, Texture texture, RectangleF textureRect, Color4 color)
    {
        if (batches.GetBatch<Quad, TexturedVertex2D>(out var batch) && currentTexture != texture)
        {
            if (currentTexture is not null)
                batch.Flush();

            currentTexture = texture;
            Graphics.SetTexture(currentTexture);
        }

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.TopLeft,
            TexCoord = new(textureRect.Left, textureRect.Top),
        });

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.BottomLeft,
            TexCoord = new(textureRect.Right, textureRect.Top),
        });

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.BottomRight,
            TexCoord = new(textureRect.Left, textureRect.Bottom),
        });

        batch.Add(new TexturedVertex2D
        {
            Color = color,
            Position = quad.TopRight,
            TexCoord = new(textureRect.Right, textureRect.Bottom),
        });
    }
}

public static class RenderContextExtensions
{
    /// <summary>
    /// Draws a texture at a given position.
    /// </summary>
    /// <param name="context">The render context.</param>
    /// <param name="texture">The texture to draw.</param>
    /// <param name="position">The position to draw at.</param>
    /// <param name="textureRect">The texture cropping.</param>
    /// <param name="color">The texture color.</param>
    public static void Draw(this RenderContext context, Texture texture, Vector2 position, RectangleF textureRect, Color4 color)
    {
        context.Draw(new RectangleF((int)position.X, (int)position.Y, texture.Width * textureRect.Width, texture.Height * textureRect.Height), texture, textureRect, color);
    }

    /// <inheritdoc cref="Draw(RenderContext, Texture, Vector2, RectangleF, Color4)"/>
    public static void Draw(this RenderContext context, Texture texture, Vector2 position, RectangleF textureRect)
    {
        Draw(context, texture, position, textureRect, Color4.White);
    }

    /// <inheritdoc cref="Draw(RenderContext, Texture, Vector2, RectangleF, Color4)"/>
    public static void Draw(this RenderContext context, Texture texture, Vector2 position)
    {
        Draw(context, texture, position, new RectangleF(0, 0, 1, 1), Color4.White);
    }
}
