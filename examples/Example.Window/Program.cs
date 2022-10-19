// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using Example.Window;
using Sekai.Engine;
using Sekai.Framework.Graphics;
using Sekai.Framework.Threading;
using Sekai.SDL;
using Sekai.Veldrid;

Game
    .Setup<ExampleGame>(new GameOptions { Graphics = { GraphicsAPI = GraphicsAPI.Vulkan, VerticalSync = true }, ExecutionMode = ExecutionMode.SingleThread })
    .UseSDLWindow()
    .UseVeldrid()
    .Run();
