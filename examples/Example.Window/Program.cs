// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Example.Window;
using Sekai.Framework;
using Sekai.Framework.Threading;
using Sekai.OpenGL;
using Sekai.SDL;

Game
    .Setup<ExampleGame>(new GameOptions { ExecutionMode = ExecutionMode.MultiThread })
    .UseSDL()
    .UseGL()
    .Run();
