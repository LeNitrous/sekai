// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Graphics;

namespace Sekai.Core.Tests;

public abstract class GraphicsDeviceTest
{
    protected GraphicsDevice Graphics = null!;

    [SetUp]
    public void SetUp()
    {
        Graphics = GraphicsDevice.Create();
    }

    [TearDown]
    public void TearDown()
    {
        Graphics.Dispose();
        Graphics = null!;
    }
}
