// Copyright (c) Cosyne and The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Sekai.Core.Tests;

[TestFixture(TickMode.Fixed)]
[TestFixture(TickMode.Variable)]
public class GameTests
{
    private Game? game;

    private readonly TickMode mode;

    public GameTests(TickMode mode)
    {
        this.mode = mode;
    }

    [SetUp]
    public void Setup()
    {
        game = new() { TickMode = mode };
    }

    [TearDown]
    public void Teardown()
    {
        game!.Exit();
        game = null;
    }

    [Test]
    public async Task Run_ShouldNotFail()
    {
        var task = Task.Run(game!.Run);

        await Task.Delay(100);

        Assert.That(task.Exception, Is.Null);
    }

    [Test]
    public async Task Run_ShouldFail_When_IsRunning()
    {
        _ = Task.Run(game!.Run);

        await Task.Delay(100);

        var task = Task.Run(game!.Run);

        await Task.Delay(100);

        Assert.That(task.Exception, Is.Not.Null);
    }

    [Test]
    public async Task RunAsync_ShouldNotFail()
    {
        var task = game!.RunAsync();

        await Task.Delay(100);

        Assert.That(task.Exception, Is.Null);
    }

    [Test]
    public async Task RunAsync_ShouldFail_When_IsRunning()
    {
        _ = game!.RunAsync();

        await Task.Delay(100);

        Assert.That(game!.RunAsync, Throws.InvalidOperationException);
    }

    [Test]
    public void Tick_ShouldNotFail()
    {
        Assert.That(game!.Tick, Throws.Nothing);
    }

    [Test]
    public async Task Tick_ShouldFail_When_IsRunning()
    {
        _ = game!.RunAsync();

        await Task.Delay(100);

        Assert.That(game!.Tick, Throws.InvalidOperationException);
    }

    [Test]
    public void UpdatePerSecond_ShouldNotThrow_When_Valid()
    {
        Assert.That(() => game!.UpdatePerSecond = 30, Throws.Nothing);
    }

    [Test]
    public void UpdatePerSecond_ShoulThrow_When_Invalid()
    {
        Assert.That(() => game!.UpdatePerSecond = -1, Throws.InstanceOf<ArgumentOutOfRangeException>());
    }
}
