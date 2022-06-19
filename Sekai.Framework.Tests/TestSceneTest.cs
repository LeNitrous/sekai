// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;

namespace Sekai.Framework.Tests;

public class TestSceneTest : FrameworkTestScene
{
    [Test]
    public void TestSceneInitialization()
    {
        Assert.That(IsLoaded, Is.True);
    }
}
