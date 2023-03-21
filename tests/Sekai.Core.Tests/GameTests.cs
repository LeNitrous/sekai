// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using NUnit.Framework;
using Sekai.Platform;

namespace Sekai.Core.Tests;

[TestFixture(TickMode.Fixed)]
[TestFixture(TickMode.Variable)]
public class GameTests
{
    private ManualGameHost? host;
    private readonly TickMode mode;

    public GameTests(TickMode mode)
    {
        this.mode = mode;
    }

    [SetUp]
    public void SetUp()
    {
        _ = new Game(host = new()) { TickMode = mode };
    }

    [TearDown]
    public void TearDown()
    {
        host = null;
    }

    [Test]
    public void Call_Create_DoesNotThrow()
    {
        Assert.Multiple(() =>
        {
            // First call does something.
            Assert.That(host!.DoCreate, Throws.Nothing);

            // Subsequent calls should throw.
            Assert.That(host.DoCreate, Throws.InvalidOperationException);
        });
    }

    [Test]
    public void Call_Load_DoesNotThrow()
    {
        // Calling before initialization should throw.
        Assert.That(host!.DoLoad, Throws.InvalidOperationException);

        host!.DoCreate();

        // Calling after initialization should throw nothing.
        Assert.That(host!.DoLoad, Throws.Nothing);
    }

    [Test]
    public void Call_Tick_DoesNotThrow()
    {
        // Calling before initialization should do nothing.
        Assert.That(host!.DoTick, Throws.Nothing);

        host.DoCreate();
        host.DoLoad();

        // Calling after initialization should do something.
        Assert.That(host!.DoTick, Throws.Nothing);

        host.DoPause();

        // Calling while paused should do nothing.
        Assert.That(host!.DoTick, Throws.Nothing);

        host.DoResume();

        // Calling after resume should do something.
        Assert.That(host!.DoTick, Throws.Nothing);
    }

    [Test]
    public void Call_Unload_DoesNotThrow()
    {
        // Calling before load should throw.
        Assert.That(host!.DoUnload, Throws.InvalidOperationException);

        host.DoCreate();
        host.DoLoad();

        // Calling after load should throw nothing.
        Assert.That(host!.DoUnload, Throws.Nothing);
    }

    [Test]
    public void Call_Destroy_DoesNotThrow()
    {
        // Calling before unload should throw.
        Assert.That(host!.DoDestroy, Throws.InvalidOperationException);

        host.DoCreate();
        host.DoLoad();
        host.DoUnload();

        // Calling after unload should throw nothing.
        Assert.That(host!.DoDestroy, Throws.Nothing);
    }
}
