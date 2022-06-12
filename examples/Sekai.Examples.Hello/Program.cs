// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Examples.Hello;
using Sekai.Framework.Platform;

using var host = Host.GetSuitableHost();
using var game = new ExampleGame();
host.Run(game);
