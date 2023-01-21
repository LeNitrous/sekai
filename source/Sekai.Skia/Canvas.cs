// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Allocation;
using Sekai.Graphics.Textures;
using Sekai.Mathematics;
using Sekai.Rendering;
using SkiaSharp;

namespace Sekai.Skia;

public abstract class Canvas : Drawable2D
{
    [Resolved]
    private SkiaContext skia { get; set; } = null!;

    /// <summary>
    /// The canvas size.
    /// </summary>
    public Size2 Size
    {
        get => size;
        set
        {
            if (size.Equals(value))
                return;

            size = value;
            recreateSurface = true;
        }
    }

    private Size2 size = new(512, 512);
    private Texture? texture;
    private SKSurface? surface;
    private bool recreateSurface = true;

    public abstract void Draw(RenderContext context, SKCanvas canvas);

    public sealed override void Draw(RenderContext context)
    {
        if (recreateSurface)
        {
            surface?.Dispose();
            surface = skia.CreateSurface(Size);

            texture?.Dispose();
            texture = Texture.New2D(Size.Width, Size.Height, PixelFormat.R8_G8_B8_A8_UNorm_SRgb);
        }

        Draw(context, surface!.Canvas);

        using (var image = surface!.Snapshot())
        using (var pixels = image.PeekPixels())
            texture!.SetData(pixels.GetPixelSpan(), 0, 0, 0, image.Width, image.Height, 1, 1, 0);

        context.Draw(new Rectangle(0, 0, Size.Width, Size.Height), texture);
    }

    public override void Unload()
    {
        surface?.Dispose();
        texture?.Dispose();
    }
}
