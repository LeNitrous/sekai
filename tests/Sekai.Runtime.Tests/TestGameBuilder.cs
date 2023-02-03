// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Null;
using Sekai.Xunit;

namespace Sekai.Runtime.Tests;

public class TestGameBuilder : ITestGameBuilder
{
    public Game Build()
    {
        return Game.Setup<TestGame>().UseNull().Build();
    }

    private class TestGame : Game
    {
    }
}
