// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Windowing;

namespace Sekai.Graphics.Tests;

[SingleThreaded]
[NonParallelizable]
public abstract class GraphicsTest<T>
    where T : GraphicsDeviceCreator, new()
{
    protected IWindow Window = null!;
    protected GraphicsDevice Device = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var creator = new T();
        Window = creator.CreateWindow();
        Device = creator.CreateGraphics(Window);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        Device.Dispose();
        Window.Dispose();
    }
}
