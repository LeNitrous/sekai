// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.IO;
using System.Runtime.InteropServices;
using Sekai.Framework.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Sekai.Engine.Graphics;

public static class Texture
{
    /// <summary>
    /// Load a texture using an image stream.
    /// </summary>
    public static ITexture Load(IGraphicsDevice graphics, Stream stream, bool mipmap = true, bool srgb = true)
    {
        return Load(graphics, Image.Load<Rgba32>(stream), mipmap, srgb);
    }

    /// <summary>
    /// Load a texture using image data.
    /// </summary>
    public static unsafe ITexture Load(IGraphicsDevice graphics, Image<Rgba32> image, bool mipmap = true, bool srgb = true)
    {
        var images = mipmap ? generateMipmaps(image) : new[] { image };

        var description = new TextureDescription
        {
            Kind = TextureKind.Texture2D,
            Usage = TextureUsage.Sampled,
            Depth = 1,
            Width = (uint)image.Width,
            Height = (uint)image.Height,
            Format = srgb ? PixelFormat.R8_G8_B8_A8_UNorm_SRgb : PixelFormat.R8_G8_B8_A8_UNorm,
            MipLevels = (uint)images.Length,
            ArrayLayers = 1,
            SampleCount = TextureSampleCount.Count1
        };

        var texture = graphics.Factory.CreateTexture(ref description);

        for (uint i = 0; i < images.Length; i++)
        {
            var img = images[i];

            if (!img.DangerousTryGetSinglePixelMemory(out var memory))
                throw new InvalidOperationException("Failed to get pixel memory.");

            fixed (void* pin = &MemoryMarshal.GetReference(memory.Span))
            {
                graphics.UpdateTextureData
                (
                    texture,
                    (nint)pin,
                    (uint)(sizeof(byte) * 4 * img.Width * img.Height),
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
