// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Graphics.Tests;

namespace Sekai.OpenGL.Tests;

[TestFixture]
public class GLGraphicsDeviceTest : GraphicsDeviceTest<GLGraphicsDeviceCreator>
{
    public GLGraphicsDeviceTest()
    {
    }
}

[TestFixture]
public class GLBufferTest : GraphicsBufferTest<GLGraphicsDeviceCreator>
{
    public GLBufferTest()
    {
    }
}

[TestFixture]
public class GLTextureTest : TextureTest<GLGraphicsDeviceCreator>
{
    public GLTextureTest()
    {
    }
}
