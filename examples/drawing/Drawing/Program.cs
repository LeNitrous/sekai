// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Drawing;
using Sekai;
using Sekai.OpenGL;
using Sekai.SDL;

Game.Setup<DrawingGame>()
    .UseSDL()
    .UseGL()
    .Run();
