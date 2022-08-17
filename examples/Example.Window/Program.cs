// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Example.Window;
using Sekai.Engine.Platform;
using Sekai.SDL;

Host
    .Setup<ExampleGame>()
    .UseWindow<SDLWindow>()
    .Run();
