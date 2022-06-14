// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework.Testing;

namespace Sekai.Framework.Tests;

public class FrameworkTestScene : TestScene
{
    protected override Game CreateGame()
    {
        return new FrameworkGame();
    }
}
