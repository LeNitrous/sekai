// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Hello;
using Sekai;
using Sekai.OpenGL;
using Sekai.SDL;

Game
    .Setup<HelloGame>()
    .UseSDL()
    .UseGL()
    .Run();
