// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;

namespace Sekai.Graphics.Textures;

public interface IRenderTarget : IDisposable
{
    int Width { get; }
    int Height { get; }
    void Bind();
    void Unbind();
}
