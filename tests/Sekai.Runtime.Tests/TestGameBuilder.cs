// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Xunit;

namespace Sekai.Runtime.Tests;

public class TestGameBuilder : ITestGameBuilder
{
    public GameBuilder Build()
    {
        return Game.Setup<TestGame>(new GameOptions { Name = @"Test" });
    }

    private class TestGame : Game
    {
    }
}
