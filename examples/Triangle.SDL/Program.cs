// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai.OpenGL;
using Sekai.SDL;
using Triangle.Game;

Sekai.Game.Setup<TriangleGame>()
    .UseSDL()
    .UseGL()
    .Run();
