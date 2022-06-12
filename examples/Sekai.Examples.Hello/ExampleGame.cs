// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.Framework;
using Sekai.Framework.Logging;

namespace Sekai.Examples.Hello;

public class ExampleGame : Game
{
    public override void Load()
    {
        Logger.Log(@"Hello World");
    }
}
