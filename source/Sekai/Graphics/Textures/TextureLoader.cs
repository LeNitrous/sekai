// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Assets;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Sekai.Graphics.Textures;

internal class TextureLoader : FrameworkObject, IAssetLoader<Texture>
{
    public string[] Extensions { get; } = new[] { ".png", ".jpg", ".jpeg" };

    public unsafe Texture Load(ReadOnlySpan<byte> bytes)
    {
        var image = Image.Load<Rgba32>(bytes);

        var texture = Texture.New2D(image.Width, image.Height, PixelFormat.R8_G8_B8_A8_UNorm);

        Span<byte> data = new byte[image.Width * image.Height * 4];
        image.CopyPixelDataTo(data);

        fixed (byte* ptr = data)
            texture.SetData((nint)ptr, 0, 0, 0, image.Width, image.Height, 1, 0, 0);

        return texture;
    }
}
