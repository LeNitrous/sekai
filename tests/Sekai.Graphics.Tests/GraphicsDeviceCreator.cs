// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using Sekai.Windowing;

namespace Sekai.Graphics.Tests;

public abstract class GraphicsDeviceCreator
{
    public abstract IWindow CreateWindow();
    public abstract GraphicsDevice CreateGraphics(IWindow window);
}
