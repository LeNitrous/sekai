// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using Sekai.Contexts;
using Sekai.GLFW;
using Sekai.Graphics;
using Sekai.Graphics.Tests;
using Sekai.Windowing;

namespace Sekai.OpenGL.Tests;

public sealed class GLGraphicsDeviceCreator : GraphicsDeviceCreator
{
    public override GraphicsDevice CreateGraphics(IWindow window)
    {
        if (window is not IGLContextSource source)
        {
            throw new ArgumentException(null, nameof(window));
        }

        return new GLGraphicsDevice(source.Context);
    }

    public override IWindow CreateWindow()
    {
        if (!RuntimeInfo.IsDesktop)
        {
            throw new PlatformNotSupportedException();
        }

        return new GLFWWindow() { Visible = false };
    }
}
