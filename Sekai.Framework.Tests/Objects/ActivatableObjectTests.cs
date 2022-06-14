// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;

namespace Sekai.Framework.Tests;

public class ActivatableObjectTests
{
    [Test]
    public void TestActivatableLifecycle()
    {
        var activatable = new TestActivatable();
        Assert.Multiple(() =>
        {
            Assert.That(() => activatable.Initialize(), Throws.Nothing);
            Assert.That(activatable.IsLoaded, Is.True);
            Assert.That(activatable.Enabled, Is.True);
            Assert.That(() => activatable.Enabled = false, Throws.Nothing);
            Assert.That(() => activatable.Enabled = true, Throws.Nothing);
            Assert.That(() => activatable.Dispose(), Throws.Nothing);
            Assert.That(activatable.IsLoaded, Is.False);
            Assert.That(activatable.Enabled, Is.False);
        });
    }

    private class TestActivatable : ActivatableObject
    {
    }
}
