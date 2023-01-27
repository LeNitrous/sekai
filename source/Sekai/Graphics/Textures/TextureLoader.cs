// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Assets;

namespace Sekai.Graphics.Textures;

internal class TextureLoader : FrameworkObject, IAssetLoader<Texture>
{
    public string[] Extensions { get; } = new[] { ".png", ".jpg", ".jpeg" };

    public unsafe Texture Load(ReadOnlySpan<byte> bytes) => Texture.Load(bytes);
}
