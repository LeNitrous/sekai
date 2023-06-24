// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Contexts;
using Sekai.Graphics;
using Sekai.Hosting;
using Sekai.Windowing;

namespace Sekai.OpenGL;

internal sealed class GLGraphicsProvider : IGraphicsProvider
{
    public GraphicsDevice CreateGraphics(IWindow window)
    {
        if (window is not IGLContextSource source)
        {
            throw new NotSupportedException("Window does not provide an OpenGL context.");
        }

        return new GLGraphicsDevice(source.Context);
    }
}
