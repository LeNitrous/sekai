// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Example.Window;
using Sekai.Engine;
using Sekai.Framework.Graphics;
using Sekai.SDL;
using Sekai.Veldrid;

Game
    .Setup<ExampleGame>(new GameOptions { Graphics = { GraphicsAPI = GraphicsAPI.Vulkan, VerticalSync = false } })
    .UseSDLWindow()
    .UseVeldrid()
    .Run();
