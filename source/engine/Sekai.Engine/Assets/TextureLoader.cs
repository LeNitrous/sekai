// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Sekai.Engine.Assets;

public sealed class TextureLoader : IAssetLoader<ITexture>
{
    private readonly IGraphicsDevice device = Game.Resolve<IGraphicsDevice>();

    public ITexture Load(ReadOnlySpan<byte> data)
    {
        var image = Image.Load<Rgba32>(data);
        var images = generateMipmaps(image);

        var description = new TextureDescription
        {
            Kind = TextureKind.Texture2D,
            Usage = TextureUsage.Sampled,
            Depth = 1,
            Width = (uint)image.Width,
            Height = (uint)image.Height,
            Format = PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
            MipLevels = (uint)images.Length,
            ArrayLayers = 1,
            SampleCount = TextureSampleCount.Count1
        };

        var texture = device.Factory.CreateTexture(ref description);

        for (uint i = 0; i < images.Length; i++)
        {
            var img = images[i];

            Span<byte> pixels = new byte[4 * img.Width * img.Height];
            img.CopyPixelDataTo(pixels);

            device.UpdateTextureData
            (
                texture,
                pixels,
                0,
                0,
                0,
                (uint)img.Width,
                (uint)img.Height,
                1,
                i,
                0
            );
        }

        return texture;
    }

    private static Image<Rgba32>[] generateMipmaps(Image<Rgba32> image)
    {
        int levels = (int)Math.Floor(Math.Log(Math.Max(image.Width, image.Height), 2));
        var images = new Image<Rgba32>[levels];
        images[0] = image;

        int i = 0;
        int w = image.Width;
        int h = image.Height;

        while (w != 1 || h != 1)
        {
            w = Math.Max(1, w / 2);
            h = Math.Max(1, h / 2);
            images[i++] = image.Clone(ctx => ctx.Resize(w, h));
        }

        return images;
    }
}
