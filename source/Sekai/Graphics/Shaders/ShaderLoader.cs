// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Text;
using Sekai.Assets;

namespace Sekai.Graphics.Shaders;

internal class ShaderLoader : FrameworkObject, IAssetLoader<Shader>
{
    public string[] Extensions { get; } = new[] { ".sksl" };

    public Shader Load(ReadOnlySpan<byte> bytes) => new(Encoding.UTF8.GetString(bytes));
}
