// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Sekai;
using Sekai.OpenGL;
using Sekai.SDL;
using TriangleAOT;

Game.Setup<TriangleGame>()
    .UseSDL()
    .UseGL()
    .Run();
