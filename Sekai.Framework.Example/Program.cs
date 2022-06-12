// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Platform;

using var host = Host.GetSuitableHost();
using var game = new ExampleGame();
host.Run(game);

internal class ExampleGame : Game
{
}
