// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Contexts;

namespace Sekai.OpenGL;

public static class GameOptionsExtensions
{
    /// <summary>
    /// Use OpenGL as the graphics provider.
    /// </summary>
    public static void UseOpenGL(this GameOptions options)
    {
        options.Graphics.CreateDevice = static (view) =>
        {
            if (view is not IGLContextSource source)
            {
                throw new NotSupportedException("The view does not provide a GL context.");
            }

            return new GLGraphicsDevice(source.Context);
        };
    }
}
